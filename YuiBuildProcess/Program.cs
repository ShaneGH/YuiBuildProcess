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
        static void Main(string[] args)
        {
            if(args.Length == 0 || args[0].ToLower() == "help")
            {
                Console.WriteLine("Syntax: YuiBuildProcess file1.js file2.js file3.js -out file5.js");
                return;
            }

            StringBuilder output = new StringBuilder();
            int i = 0;
            for (; i < args.Length && !args[i].StartsWith("-"); i++)
            {
                output.AppendLine(File.ReadAllText(args[i]));
                output.Append(";");
            }

            if (i != args.Length - 2 || args[i] != "-out")
            {
                Console.WriteLine("Syntax: YuiBuildProcess file1.js file2.js file3.js -out file5.js");
                return;
            }

            Compressor c = new JavaScriptCompressor();
            File.WriteAllText(args[i + 1], c.Compress(output.ToString()));
        }
    }
}
