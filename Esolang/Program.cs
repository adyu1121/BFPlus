using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace Esolang
{
    class MainClass
    {
        static string Version = "1.1.0";
        static string versionBFP = "1.0";
        static void Help()
        {
            Console.WriteLine();
            Console.WriteLine("Usage: BFPRuner [FileName.extension]");
            //Console.WriteLine("    or BFPRuner [RunType] [FileName.extension] [...Options]");
            Console.WriteLine("    or BFPRuner [RunType] [FileName.extension]");
            Console.WriteLine("    or BFPRuner [Help]");
            Console.WriteLine("    (If the File extension is .bfp, you can don't have to write the extension name)");
            Console.WriteLine();
            Console.WriteLine("RunType: interpret or Compile(default:interpret)");
            Console.WriteLine("    interpret:Run The File[FileName.extension] in CMD");
            Console.WriteLine();
            Console.WriteLine("    Compile:Compile The File[FileName.extension] to [FileName].bf");
            Console.WriteLine("    [FileName].bf create location equals File[FileName.extension] location");
            Console.WriteLine();
            Console.WriteLine("Help Command List");
            Console.WriteLine("    -help:Print Command list and Help");
            Console.WriteLine("    -version:Print BFPRuner version");
            Console.WriteLine("    -versionBFP:Print Corresponding BFP version");
            /*Console.WriteLine("Options List");
            Console.WriteLine("    -");*/
            //Console.WriteLine("\t(If File extension is .bfp, you can don't have to write extensimove A to Bon name)");
        }
        static void interpret(string path) 
        {
            string Path = path;
            if (!Path.Contains('.'))
            {
                Path += ".bfp";
            }
            if (File.Exists(Path))
            {
                /*using (FileStream fs = new FileStream($"{argv[0]}", FileMode.Open, FileAccess.Read))
                {
                    Interpreter interpreter = new Interpreter(fs);
                    interpreter.RunCode();
                }*/
                using FileStream fs = new FileStream($"{Path}", FileMode.Open, FileAccess.Read);

                Interpreter interpreter = new Interpreter(fs);
                interpreter.RunCode();
            }
            else
            {
                Console.WriteLine($"Not Found File \"{Path}\"");
            }
        }

        //static StreamReader sr;
        public static void Main(string[] argv)
        {
            if (argv.Length == 0)
            {
                Help();
            }
            else
            {
                switch (argv[0])
                {
                    case "-help":
                        Help();
                        break;
                    case "-version":
                        Console.WriteLine($"{Version}");
                        break;
                    case "-versionBFP":
                        Console.WriteLine($"{versionBFP}");
                        break;
                    case "interpret":
                        interpret(argv[1]);
                        break;
                    case "Compile":
                        break;
                    default:
                        interpret(argv[0]);
                        break;
                }
            }
        }
    }
}
    
