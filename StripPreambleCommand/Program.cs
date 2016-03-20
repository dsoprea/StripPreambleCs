using StripPreamble;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripPreambleCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Count() < 3)
            {
                Console.WriteLine("Please provide a preamble type-name, separator prefix, and at least one file/pattern.");
                Environment.Exit(1);
            }

            string typeName = args[0];

            if(CommonMatchers.Matchers.ContainsKey(typeName) == false)
            {
                Console.WriteLine("Matcher [{0}] is not valid.", typeName);
                Environment.Exit(2);
            }

            PreambleMatcher pm = CommonMatchers.Matchers[typeName];

            string separatorPrefix = args[1];

            IList<string> filepaths = new List<string>();

            int i = 2;
            while(i < args.Count())
            {
                string filepath = args[i];
                string path = Path.GetFullPath(Path.GetDirectoryName(filepath));
                string pattern = Path.GetFileName(filepath);

                string[] matched = Directory.GetFiles(path, pattern);
                if(matched.Count() == 0)
                {
                    throw new FileNotFoundException(String.Format("No files found like [{0}] in path [{1}].", pattern, path));
                }

                foreach(string filename in matched)
                {
                    filepaths.Add(Path.Combine(path, filename));
                }

                i++;
            }

            Concatenate c = new Concatenate();
            IList<string> output = c.Run(pm, filepaths, separatorPrefix: separatorPrefix);
            foreach(string line in output)
            {
                Console.WriteLine(line);
            }
        }
    }
}
