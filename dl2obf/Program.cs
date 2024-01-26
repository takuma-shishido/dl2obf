using Mono.Cecil;
using System;
using System.Diagnostics;
using System.IO;

namespace dl2obf
{
    class Program
    {
        public static string FileName;
        public static string FilePath;
        public static string FileDirectory;

        private static AssemblyDefinition _assemblyDef;
        private static Obfuscator _obfuscator;
        private static Stopwatch _timer;

        private static void InitConsole()
        {
            Console.Title = "dl2obf";
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void PrintDoneMessage()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("Total Time: " + _timer.Elapsed.ToString(@"s\.ff") + "s");
            Console.WriteLine($@"DONE");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void ObfuscateAllMethods(TypeDefinition t)
        {
            if (t.HasMethods)
            {
                Console.WriteLine($"\nObfuscating all Methods For '{t.Name}'..");
                foreach (var m in t.Methods)
                {
                    if (m.IsConstructor | m.IsRuntime | m.IsRuntimeSpecialName | m.IsVirtual | m.IsAbstract | (m.GenericParameters != null && m.GenericParameters.Count > 0) | m.Overrides.Count > 0 | m.Name.StartsWith("<") || m.Name.ToLower().StartsWith("do")) continue;

                    if (m.HasBody)
                    {
                        _obfuscator.ObfuscateStrings(m);
                    }

                    _obfuscator.ObfuscateMethodName(m);

                }
            }
        }

        private static void ObfuscateAllFields(TypeDefinition t)
        {
            if (t.HasFields)
            {
                Console.WriteLine($"\nObfuscating all Fields {t.Name}..");

                foreach (var f in t.Fields)
                {
                    if (f.IsRuntimeSpecialName | f.IsSpecialName | f.Name.StartsWith("<")) continue;
                    _obfuscator.ObfuscateFieldName(f);
                }
            }
        }

        private static void ObfuscateAllProperties(TypeDefinition t)
        {
            Console.WriteLine($"\nObfuscating all Properties {t.Name}..");

            foreach (var p in t.Properties)
            {
                if (p.IsSpecialName | p.IsRuntimeSpecialName) continue;

                _obfuscator.ObfuscatePropertyName(p);
            }
        }

        private static void ObfuscateFile()
        {
            foreach (TypeDefinition t in _assemblyDef.MainModule.Types)
            {
                if (t.Name.StartsWith("<")) continue;

                ObfuscateAllFields(t);

                ObfuscateAllMethods(t);

                ObfuscateAllProperties(t);

                _obfuscator.ObfuscateClassName(t);
            }
        }

        private static int Init()
        {
            _obfuscator = new Obfuscator();
            _timer = new Stopwatch();

            FilePath = Console.ReadLine();
            if (FilePath == null) return 0;

            FilePath = FilePath.Replace("\"", "");

            if (File.Exists(FilePath))
            {
                FileName = Path.GetFileName(FilePath);
            }
            else if (Path.GetExtension(FilePath) != ".dll")
            {
                Console.WriteLine("Inserted file isn't of a .dll or .exe file type.");
                return 0;
            }
            else
            {
                Console.WriteLine("Inserted File path could not be found. Try dragging the file into the console.");
                return 0;
            }

            FileDirectory = FilePath.Replace(FileName ?? string.Empty, "");

            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(FileDirectory);

            _assemblyDef =
                AssemblyDefinition.ReadAssembly(FilePath, new ReaderParameters { AssemblyResolver = resolver });

            return 1;
        }

        static void Main()
        {
            InitConsole();
            Console.WriteLine("How to use: simply drag a file or paste a filepath in the console and press enter.");
            while (true)
            {
                Console.Write("> ");
                if (Init() == 1)
                    break;
            }

            _timer.Start();
            ObfuscateFile();
            _timer.Stop();

            PrintDoneMessage();

            string newFilePath = $"{FileDirectory}obfuscated_{FileName}";
            _assemblyDef.Write(newFilePath);
            Console.WriteLine(newFilePath);
            Console.WriteLine("\nPress enter to close the program\n");
        }
    }
}
