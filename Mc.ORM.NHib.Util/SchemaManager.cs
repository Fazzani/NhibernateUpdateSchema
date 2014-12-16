//  David Newman <newman.de@gmail.com>
//  Guilherme Balena Versiani <guibv@yahoo.com>
//
// Copyright (C) 2010 David Newman
// Copyright (C) 2011 Guilherme Balena Versiani
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Data;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Mc.ORM.NHib.Util
{
    /// <summary>
    /// Wraps the SchemaExport and SchemaUpdate tools from NHibernate, providing a
    /// single easy to use class to manage database schemas.
    /// </summary>
    public class SchemaManager
    {
        //private string ConnectionString = "Server=local;User ID=leetchi;Password=leetchi;Database=leetchi;Trusted_Connection=False;";
        //private const string connectionStringKey = "connection.connection_string";
        /// <summary>
        /// Stores assemblies for future resolution
        /// </summary>
        private Dictionary<string, Assembly> AssemblyCache { get; set; }
        private FluentConfiguration _fluentConfiguration;

        public FluentConfiguration FluentConfiguration
        {
            get
            {
                if (_fluentConfiguration == null)
                {
                    _fluentConfiguration =
                        Fluently.Configure()
                            .ProxyFactoryFactory<NHibernate.ByteCode.Castle.ProxyFactoryFactory>();
                }
                return _fluentConfiguration;
            }
        }

        /// <summary>
        /// Initialized ISessionFactory
        /// </summary>
        /// 
        private ISessionFactory _sessionFactory;
        protected ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    if (Configuration == null)
                        throw new ArgumentException("Could not create ISessionFactory because there is no Configuration");
                    
                    _sessionFactory = DefaultConfiguration.BuildSessionFactory();
                }

                return _sessionFactory;
            }
        }

        private Configuration _configuration;
        /// <summary>
        /// NHibernate Congfiguration
        /// </summary>
        protected Configuration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = FluentConfiguration.BuildConfiguration();
                    _configuration.SetProperty("connection.connection_string", ConfigurationHelper.GetConnectionString(Options.ConnectionStringKey, Options.WorkingDirectory));
                    _configuration.SetProperty("dialect", "NHibernate.Dialect.MsSql2005Dialect");
                    _configuration.SetProperty("show_sql", "false");
                    _configuration.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
                    _configuration.SetProperty("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
                    _configuration.SetProperty("connection.release_mode", "auto");
                    _configuration.SetProperty("adonet.batch_size", "500");
                    _configuration.SetProperty("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
                }
                
                return _configuration;
            }
        }

        protected Configuration DefaultConfiguration
        {
            get
            {
                var configuration = new Configuration();
                configuration.SetProperty("connection.connection_string",
                    ConfigurationHelper.GetConnectionString(Options.ConnectionStringKey, Options.WorkingDirectory));
                configuration.SetProperty("dialect", "NHibernate.Dialect.MsSql2005Dialect");
                configuration.SetProperty("show_sql", "false");
                configuration.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
                configuration.SetProperty("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
                configuration.SetProperty("connection.release_mode", "auto");
                configuration.SetProperty("adonet.batch_size", "500");
                configuration.SetProperty("proxyfactory.factory_class",
                    "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
                return configuration;
            }
        }

        /// <summary>
        /// Options that drive how schema is exported or updated
        /// </summary>
        protected SchemaManagerOptions Options { get; set; }

        /// <summary>
        /// Initializes a new SchemaManager instance
        /// </summary>
        /// <param name="options">Options that drive how schema is exported or updated</param>
        public SchemaManager(SchemaManagerOptions options)
        {
            //if (string.IsNullOrEmpty(options.ConfigFile))
            //    options.ConfigFile = Path.Combine(options.WorkingDirectory, "hibernate.cfg.xml");
            //Configuration = new Configuration();
            //try
            //{
            //    Configuration.Configure(options.ConfigFile);

            //}
            //catch (Exception)
            //{
            //    throw new InvalidDataException("ConfigFile not valid");
            //}

            Options = options;

            AssemblyCache = new Dictionary<string, Assembly>();

            if (options.ModelAssemblies != null && options.ModelAssemblies.Any())
                AddModelsToDomain();

            if (options.MappingAssemblies != null && options.MappingAssemblies.Any())
                AddAssemblyMappings();

            if (options.MappingDirectories != null && options.MappingDirectories.Any())
                AddMappingsFromDirectories();

            if (options.FluentAssemblies != null && options.FluentAssemblies.Any())
                AddFluentMappingsFromAssemblies();
        }

        /// <summary>
        /// If Options.Mode is Execute then this method will generate the correct DDL script
        /// and execute it against the database.  If Options.Mode is Silent then this method
        /// will only generate the correct DDL script.
        /// </summary>
        /// <returns>string containing the DDL script for updating the schema</returns>
        public string Update()
        {
            var sqlText = new StringBuilder();

            try
            {

                var updater = Options.Settings != null
                    ? new SchemaUpdate(Configuration, Options.Settings)
                    : new SchemaUpdate(Configuration);

                Action<string> lineAction = line => sqlText.AppendLine(line);

                using (var session = SessionFactory.OpenSession())
                {
                    using (var trans = session.BeginTransaction())
                    {
                        switch (Options.Mode)
                        {
                            case SchemaManagerMode.Execute:
                                updater.Execute(lineAction, true);
                                break;
                            case SchemaManagerMode.Silent:
                                updater.Execute(lineAction, false);
                                break;
                            default:
                                throw new InvalidOperationException("The value of SchemaManager.Options.Mode is unknown");
                        }
                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SchemaManagerException("Error updating schema: " + ex.Message, sqlText.ToString(), ex);
            }

            return sqlText.ToString();
        }

        /// <summary>
        /// Drops the schema from the database
        /// </summary>
        /// <returns>DDL script for dropping the schema</returns>
        public string Drop()
        {
            var sqlText = new StringBuilder();

            try
            {
                var schema = new SchemaExport(Configuration);

                using (var session = SessionFactory.OpenSession())
                {
                    using (var trans = session.BeginTransaction())
                    {
                        Action<string> lineAction = line => sqlText.AppendLine(line);

                        switch (Options.Mode)
                        {
                            case SchemaManagerMode.Execute:
                                schema.Execute(lineAction, true, true);
                                break;

                            case SchemaManagerMode.Silent:
                                schema.Execute(lineAction, false, true);
                                break;

                            default:
                                throw new InvalidOperationException("The value of SchemaManager.Options.Mode is unknown");
                        }

                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SchemaManagerException("Error dropping schema: " + ex.Message, sqlText.ToString(), ex);
            }

            return sqlText.ToString();
        }

        /// <summary>
        /// Creates the schema in the database
        /// </summary>
        /// <returns>DDL script used to create the schema</returns>
        public string Create()
        {
            var sqlText = new StringBuilder();

            try
            {
                var schema = new SchemaExport(Configuration);

                using (var session = SessionFactory.OpenSession())
                {
                    using (var trans = session.BeginTransaction())
                    {
                        Action<string> lineAction = line => sqlText.AppendLine(line);

                        switch (Options.Mode)
                        {
                            case SchemaManagerMode.Execute:
                                schema.Execute(lineAction, true, false);
                                break;

                            case SchemaManagerMode.Silent:
                                schema.Execute(lineAction, false, false);
                                break;

                            default:
                                throw new InvalidOperationException("The value of SchemaManager.Options.Mode is unknown");
                        }

                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SchemaManagerException("Error creating schema: " + ex.Message, sqlText.ToString(), ex);
            }

            return sqlText.ToString();
        }

        /// <summary>
        /// Add datasets to current database.
        /// </summary>
        public void AddDatasets()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                using (var session = SessionFactory.OpenSession())
                {
                    using (var trans = session.BeginTransaction())
                    {
                        if (Options.CsvDatasets != null && Options.CsvDatasets.Any())
                            AddDatasetFromCsvFiles(session);

                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SchemaManagerException("Error adding dataset: " + ex.Message, "", ex);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
        }

        protected void AddDatasetFromCsvFiles(ISession session)
        {
            foreach (var file in Options.CsvDatasets.Select(x => new FileInfo(x)))
            {
                var reader = new CsvReader(';');
                var dataset = reader.Read(file);
                DatasetUtils.Save(dataset, session);
            }
        }

        /// <summary>
        /// Adds mappings embedded in a set of assemblies to the configuration
        /// </summary>
        protected void AddAssemblyMappings()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                foreach (var mappings in Options.MappingAssemblies.Select(Assembly.LoadFrom))
                {
                    AssemblyCache[mappings.FullName] = mappings;
                    AssemblyCache[mappings.GetName().Name] = mappings;

                    Configuration.AddAssembly(mappings);
                }
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
        }

        protected Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return AssemblyCache.ContainsKey(args.Name) ? AssemblyCache[args.Name] : null;
        }

        protected void AddMappingsFromDirectories()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                foreach (var dir in Options.MappingDirectories)
                {
                    Configuration.AddDirectory(new System.IO.DirectoryInfo(dir));
                }
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
        }

        private void AddFluentMappingsFromAssemblies()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                Options.FluentAssemblies
                    .Select(x => Assembly.LoadFile(Path.GetFullPath(x)))
                    .Aggregate(new MappingConfiguration(), (m, a) =>
                    {
                        m.FluentMappings.AddFromAssembly(a);
                        m.FluentMappings.Conventions.Add(
                            // foreign key column name (one-to-many or many-to-one relation column)
                                ForeignKey.EndsWith("Id"),
                            // foreign key constraint naming convention for many-to-one
                                ConventionBuilder.Reference.Always(instance => instance.ForeignKey(
                                    String.Format("FK_{0}_{1}Id", instance.EntityType.Name, instance.Property.Name)))
                            );
                        return m;
                    })
                    .Apply(Configuration);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
        }

        /// <summary>
        /// Adds any model classes to the app domain
        /// </summary>
        protected void AddModelsToDomain()
        {
            foreach (var models in Options.ModelAssemblies.Select(Assembly.LoadFrom))
            {
                AssemblyCache[models.FullName] = models;
                AssemblyCache[models.GetName().Name] = models;
            }
        }
    }
}
