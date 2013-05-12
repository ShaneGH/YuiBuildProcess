using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yahoo.Yui.Compressor;
using System.IO;

namespace YuiBuildProcess
{
    class Program
    {
        static readonly string[] ValidCommands = new[] { "-out", "-compress" };
        const string Syntax = "Syntax: YuiBuildProcess file1.js file2.js file3.js -out file5.js [-compress true|false]";
        static void Main(string[] args)
        {
            if(args.Length == 0 || args[0].ToLower() == "help")
            {
                Console.WriteLine(Syntax);
                return;
            }

            StringBuilder output = new StringBuilder();
            int i = 0;
            for (; i < args.Length && !args[i].StartsWith("-"); i++)
            {
                output.AppendLine(File.ReadAllText(args[i]));
                output.AppendLine();
            }

            args = args.Skip(i).ToArray();
            if (args.Length % 2 != 0)
            {
                Console.WriteLine(Syntax);
                return;
            }

            Dictionary<string, string> commands = new Dictionary<string, string>();
            for (var j = 0; j < args.Length; j += 2)
            {
                if (!ValidCommands.Contains(args[j]))
                {
                    Console.WriteLine(Syntax);
                    return;
                }

                commands.Add(args[j], args[j + 1]);
            }

            if (!commands.ContainsKey("-out"))
            {
                Console.WriteLine(Syntax);
                return;
            }

            // default is true
            bool compress = true;
            if (commands.ContainsKey("-compress") && !bool.TryParse(commands["-compress"], out compress))
            {
                Console.WriteLine(Syntax);
                return;
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

            File.WriteAllText(commands["-out"], compressF());
        }
    }
}
