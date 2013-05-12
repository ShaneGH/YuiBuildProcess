using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yahoo.Yui.Compressor;
using System.IO;
using System.Diagnostics;

namespace YuiBuildProcess
{
    class Program
    {
        static readonly string[] ValidCommands = new[] { "-out", "-compress" };
        const string Syntax = "Syntax: YuiBuildProcess file1.js file2.js directory1 -out file5.js [-compress true|false]\n\tIf order is important add order specific files first. They will be ignored if a directory is scanned later";
        static int Main(string[] args)
        {
            args = new[] 
            {
                @"C:\Dev\Apps\BackToFront\BackToFront.Web\Scripts\BackToFront\Expressions\Expression.js",
                @"C:\Dev\Apps\BackToFront\BackToFront.Web\Scripts\BackToFront", 
                @"C:\Dev\Apps\BackToFront\BackToFront.Web\Scripts\linq-0.0.js",     
                @"-out", 
                @"hello.js" 
            };

            if (args.Length == 0 || (args.Length == 1 && args[0].ToLower() == "help"))
            {
                Console.WriteLine(Syntax);
                return 0;
            }

            var arguments = new List<string>(args);

            var added = new List<string>();
            StringBuilder output = new StringBuilder();

            int i = 0;
            for (; i < arguments.Count && !arguments[i].StartsWith("-"); i++)
            {
                FileAttributes attr = File.GetAttributes(arguments[i]);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    var di = new DirectoryInfo(arguments[i]);
                    if (!added.Contains(di.FullName))
                    {
                        added.Add(di.FullName);
                        arguments.InsertRange(i + 1, di.GetDirectories().Select(d => d.FullName));
                        arguments.InsertRange(i + 1, di.GetFiles().Select(d => d.FullName).Where(n => n != null && n.ToLower().EndsWith(".js")));
                    }
                }
                else
                {
                    var fi = new FileInfo(arguments[i]);
                    if (!added.Contains(fi.FullName))
                    {
                        added.Add(fi.FullName);
                        output.AppendLine(File.ReadAllText(arguments[i]));
                        output.AppendLine();
                    }
                }
            }

            arguments = arguments.Skip(i).ToList();
            if (arguments.Count % 2 != 0)
            {
                Console.WriteLine(Syntax);
                return 1;
            }

            Dictionary<string, string> commands = new Dictionary<string, string>();
            for (var j = 0; j < arguments.Count; j += 2)
            {
                if (!ValidCommands.Contains(arguments[j]))
                {
                    Console.WriteLine(Syntax);
                    return 1;
                }

                commands.Add(arguments[j], arguments[j + 1]);
            }

            if (!commands.ContainsKey("-out"))
            {
                Console.WriteLine(Syntax);
                return 1;
            }

            // default is true
            bool compress = true;
            if (commands.ContainsKey("-compress") && !bool.TryParse(commands["-compress"], out compress))
            {
                Console.WriteLine(Syntax);
                return 1;
            }

            Func<string> compressF = () =>
            {
                if (compress)
                {
                    Compressor c = new JavaScriptCompressor();
                    return c.Compress(output.ToString());
                }

                return output.ToString();
            };

            using (var fs = new FileStream(commands["-out"], FileMode.Create))
            {
                using (var w = new StreamWriter(fs))
                {
                    w.Write(compressF());
                }
            }

            return 0;
        }
    }
}
