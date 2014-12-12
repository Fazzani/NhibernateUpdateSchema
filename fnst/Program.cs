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
using System.Linq;
using System.IO;

using Mono.Options;

using Mc.ORM.NHib.Util;

namespace fnst
{
    class Program
    {

        private static Operation _operation = Operation.Create;
        private static bool _verbose = false;
        private static bool _debug = true;
        private static bool _license = false;
        private static bool _help = false;

        static void Main(string[] args)
        {
            try
            {
                var options = new SchemaManagerOptions();
                var os = RegisterOptions(options);

                if (args.Length == 0)
                {
                    PrintNoArgs();
                    return;
                }

                os.Parse(args);                

                if (_debug)
                {
                    PrintOptions(options);
                }

                if (_license)
                {
                    PrintResourceFile("fnst.license.txt");
                }
                else if (_help)
                {
                    PrintResourceFile("fnst.HelpCommand.txt");
                }
                else
                {
                    Execute(options);
                }
                if (_debug)
                {
                    Console.WriteLine("The end");
                    Console.ReadKey();
                }

            } 
            catch (Exception ex)
            {
                PrintError(ex);
            }
        }

        static OptionSet RegisterOptions(SchemaManagerOptions options)
        {
            var os = new OptionSet();
            
            os.Add("c:", c => options.ConfigFile = Path.GetFullPath(c));
            os.Add("a:", a => options.MappingAssemblies = a.Split(';').Select(Path.GetFullPath).ToList());
            os.Add("d:", d => options.MappingDirectories = d.Split(';').Select(Path.GetFullPath).ToList());
            os.Add("f:", f => options.FluentAssemblies = f.Split(';').Select(Path.GetFullPath).ToList());
            os.Add("m:", m => options.ModelAssemblies = m.Split(';').Select(Path.GetFullPath).ToList());
            os.Add("g:", g => options.CsvDatasets = g.Split(';').Select(Path.GetFullPath).ToList());
            os.Add("s", s => options.Mode = (s != null) ? SchemaManagerMode.Silent : SchemaManagerMode.Execute);
            os.Add("o:", o => _operation = (Operation)Enum.Parse(typeof(Operation), o));
            os.Add("v", v => _verbose = (v != null));
            os.Add("b", b => _debug = (b != null));
            os.Add("L", L => _license = (L != null));
            os.Add("h|?", h => _help = (h != null));

            return os;
        }

        static void Execute(SchemaManagerOptions options)
        {
            var sm = new SchemaManager(options);
            var script = string.Empty;

            switch (_operation)
            {
                case Operation.Create:
                    script = sm.Create();
                    break;
                case Operation.Update:
                    script = sm.Update();
                    break;
                case Operation.Delete:
                    script = sm.Drop();
                    break;
            }

            if (!string.IsNullOrEmpty(script))
            {
                if (options.Mode == SchemaManagerMode.Silent || _verbose)
                    Console.WriteLine(script);                
            }

            if (options.CsvDatasets != null && options.CsvDatasets.Any())
                sm.AddDatasets();
        }

        static void PrintError(Exception ex)
        {
            Console.WriteLine("Error encountered: " + ex.Message);
        }

        static void PrintOptions(SchemaManagerOptions options)
        {
            Console.WriteLine(options.ConfigFile);

            if(options.MappingAssemblies != null)
                foreach (var a in options.MappingAssemblies)
                    Console.WriteLine(a);

            if (options.MappingDirectories != null)
                foreach (var d in options.MappingDirectories)
                    Console.WriteLine(d);

            if(options.ModelAssemblies != null)
                foreach (var m in options.ModelAssemblies)
                    Console.WriteLine(m);

            if (options.FluentAssemblies != null)
                foreach (var a in options.FluentAssemblies)
                    Console.WriteLine(a);

            Console.WriteLine("Embedded Resources");

            var asm = Type.GetType("fnst.Program").Assembly;
            foreach (var name in asm.GetManifestResourceNames())
            {
                Console.WriteLine(name);
            }
        }

        static void PrintResourceFile(string fileName)
        {
            var asm = Type.GetType("fnst.Program").Assembly;

            using (var sr = new StreamReader(asm.GetManifestResourceStream(fileName)))
            {
                Console.WriteLine(sr.ReadToEnd());
            }
        }

        static void PrintNoArgs()
        {
            PrintResourceFile("fnst.HelpCommand.txt");
        }
    }

    internal enum Operation
    {
        Create = 0,
        Update,
        Delete
    }
}
