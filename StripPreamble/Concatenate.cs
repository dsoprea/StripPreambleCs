using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripPreamble
{
    public class Concatenate
    {
        public Concatenate()
        {

        }

        public IList<string> Run(PreambleMatcher pm, IList<string> filepaths, string separatorPrefix = null)
        {
            IList<string> aggregate = new List<string>();

            foreach (string filepath in filepaths)
            {
                Console.Error.WriteLine("Reading: [{0}]", filepath);

                IList<string> lines = File.ReadAllLines(filepath).ToList();
                pm.TryStrip(lines);

                if(separatorPrefix != null)
                {
                    aggregate.Add(separatorPrefix + filepath);
                    aggregate.Add("");
                    aggregate.Add("");
                }

                foreach (string line in lines)
                {
                    aggregate.Add(line);
                }

                aggregate.Add("");
                aggregate.Add("");
            }

            return aggregate;
        }
    }
}
