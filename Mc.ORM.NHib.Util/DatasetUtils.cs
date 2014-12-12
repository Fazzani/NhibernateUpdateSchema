using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NHibernate;

namespace Mc.ORM.NHib.Util
{
    internal static class DatasetUtils
    {
        public static void Save(IEnumerable<IList<string>> dataset, ISession session)
        {
            var currentLine = 1;
            Type currentType = null;
            var expectHeaderNextLine = false;
            IList<string> currentHeader = null;
            var entities = new Dictionary<string, object>();

            foreach (var row in dataset)
            {
                if (Regex.Match(row[0], "^@type *= *").Success)
                {
                    var m = Regex.Match(row[0], "^@type *= *(?<Type>.*)$");
                    if (!m.Success)
                        throw new InvalidDataException(string.Format("Type spec error at line {0}", currentLine));
                    var typeName = m.Groups["Type"].Value;
                    currentType = Type.GetType(typeName);
                    if (currentType == null)
                        throw new InvalidDataException(string.Format("Type not found at line {0}", currentLine));
                    currentHeader = null;
                    expectHeaderNextLine = true;
                }
                else if (expectHeaderNextLine)
                {
                    if (row.Count == 0)
                        throw new InvalidDataException(string.Format("Expected header at line {0}", currentLine));
                    currentHeader = row;
                    expectHeaderNextLine = false;
                }
                else
                {
                    if (currentType == null)
                        throw new InvalidDataException(string.Format("Expected '@type=' declaration before line {0}", currentLine));
                    if (currentHeader == null)
                        throw new InvalidDataException(string.Format("Expected header before line {0}", currentLine));
                    SaveRow(currentType, currentHeader, row, currentLine, session, entities);
                }

                currentLine++;
            }
        }

        public static void SaveRow(Type instanceType, IList<string> header,
            IList<string> row, int currentLine, ISession session,
            IDictionary<string, object> entities)
        {
            var id = string.Empty;
            var instance = Activator.CreateInstance(instanceType);

            for (var i = 0; i < header.Count; i++)
            {
                if (string.Compare(header[i], "@Object_Id", true) == 0)
                {
                    if (!string.IsNullOrEmpty(row[i]))
                        id = row[i];
                    continue;
                }

                var property = instanceType.GetProperty(header[i]);
                object value;

                if (Regex.Match(row[i], "^@[a-zA-Z0-9.-_]+@$").Success)
                {
                    var m = Regex.Match(row[i], "^@(?<Id>.*)@$");
                    value = entities[m.Groups["Id"].Value];
                    if (value == null)
                        throw new InvalidDataException(string.Format("Invalid reference at line {0}", currentLine));
                }
                else if (Regex.Match(row[i], "^@query *=.*@$").Success)
                {
                    var m = Regex.Match(row[i], "^@query *= *(?<Query>.*)@$");
                    value = session.CreateQuery(m.Groups["Query"].Value).UniqueResult();
                    if (value == null)
                        throw new InvalidDataException(string.Format("Invalid reference at line {0}", currentLine));
                }
                else
                {
                    var propertyType = property.PropertyType;
                    value = DataConverter.Convert(propertyType, row[i]);
                }

                property.SetValue(instance, value, null);
            }

            session.Save(instance);

            if (!string.IsNullOrEmpty(id))
            {
                entities[id] = instance;
            }
        }
    }
}
