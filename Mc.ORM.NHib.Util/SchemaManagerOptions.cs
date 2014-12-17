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

using System.Collections.Generic;
using NHibernate.Cfg;

namespace Mc.ORM.NHib.Util
{
    /// <summary>
    /// Allows clients to provide the export/update options necessary to
    /// drive the SchemaExport and SchemaUpdate tools.
    /// </summary>
    public class SchemaManagerOptions
    {
        /// <summary>
        /// Configuration connection string Name
        /// </summary>
        public string ConfigName { get; set; }
        /// <summary>
        /// Model Directory
        /// </summary>
        public string ModelDirectory { get; set; }
        /// <summary>
        /// Configuration Directory
        /// </summary>
        public string ConfigDirectory { get; set; }
        /// <summary>
        /// Connection string key
        /// </summary>
        public string ConnectionStringKey { get; set; }
        /// <summary>
        /// Connection string key value
        /// </summary>
        public string ConnectionStringKeyValue { get; set; }
        /// <summary>
        /// Path to the NHibernate configuration file
        /// </summary>
        public string ConfigFile { get; set; }

        /// <summary>
        /// Sql file name output
        /// </summary>
        public string SQLFileName { get; set; }

        /// <summary>
        /// Allows clients to specify overrides to the default NHibernate settings
        /// </summary>
        public Settings Settings { get; set; }

        private SchemaManagerMode _mode = SchemaManagerMode.Execute;
        /// <summary>
        /// Client can set the mode in which SchemaManager operations will work.
        /// Default value is SchemaManagerMode.Execute
        /// </summary>
        public SchemaManagerMode Mode { get { return _mode; } set { _mode = value; } }

        /// <summary>
        /// List of assembly names that contain embedded mapping files
        /// </summary>
        public IEnumerable<string> MappingAssemblies { get; set; }

        /// <summary>
        /// List of assembly names that contain the actual model objects. This allows for 
        /// cases where the Domain Model classes are not stored in the same assembly as
        /// the embedded mapping files, or if the mapping files are somehow loaded from an
        /// external source.  These assemblies are loaded into the application domain prior to 
        /// adding the mappings to the NHibernate Configuration object.
        /// </summary>
        public IEnumerable<string> ModelAssemblies { get; set; }

        /// <summary>
        /// The set of directories that contain NHibernate .hbm.xml files. This can
        /// be used in conjunction with ModelAssemblies in order to load mapping files
        /// from the filesystem, as oppossed to from an assembly.
        /// </summary>
        public IEnumerable<string> MappingDirectories { get; set; }

        /// <summary>
        /// List of assembly names that contain fluent mappings.
        /// </summary>
        public IEnumerable<string> FluentAssemblies { get; set; }

        /// <summary>
        /// List of CSV files containing datasets.
        /// </summary>
        public IList<string> CsvDatasets { get; set; }
    }

    /// <summary>
    /// Identifies the mode in which the SchemaManger will execute its operations
    ///
    /// </summary>
    public enum SchemaManagerMode
    {
        /// <summary>
        /// Sql Commands will be executed against the database
        /// </summary>
        Execute,

        /// <summary>
        /// Sql Commands will not be executed, but will generate script text
        /// </summary>
        Silent
    }
}
