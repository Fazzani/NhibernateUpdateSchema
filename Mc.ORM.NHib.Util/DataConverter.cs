using System;
using System.Globalization;
using System.IO;

namespace Mc.ORM.NHib.Util
{
    internal class DataConverter
    {
        public static object Convert(Type type, string propertyValue)
        {
            if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return Guid.Parse(propertyValue);
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                var d = DateTime.UtcNow;
                DateTime.TryParseExact(propertyValue, @"yyyy-MM-dd\THH:mm:ss\Z",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out d);
                return d;
            }
            else if (type == typeof(Int32) || type == typeof(Int32?))
            {
                return Int32.Parse(propertyValue);
            }
            else if (type == typeof(UInt32) || type == typeof(UInt32?))
            {
                return UInt32.Parse(propertyValue);
            }
            else if (type == typeof(Int64) || type == typeof(Int64?))
            {
                return Int64.Parse(propertyValue);
            }
            else if (type == typeof(UInt64) || type == typeof(UInt64?))
            {
                return UInt64.Parse(propertyValue);
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                return int.Parse(propertyValue);
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                return float.Parse(propertyValue);
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                return double.Parse(propertyValue);
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                return bool.Parse(propertyValue);
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                return long.Parse(propertyValue);
            }
            else if (type == typeof(byte) || type == typeof(byte?))
            {
                return byte.Parse(propertyValue);
            }
            else if (type == typeof(string))
            {
                return propertyValue;
            }
            else if (type == typeof(Uri))
            {
                return new Uri(propertyValue);
            }
            else if (type.IsEnum)
            {
                return Enum.ToObject(type, int.Parse(propertyValue));
            }
            else
            {
                throw new InvalidDataException(string.Format("Unknown data type {0}", type));
            }
        }
    };
}
