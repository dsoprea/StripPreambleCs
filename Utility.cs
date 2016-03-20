using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripPreamble
{
    public class Utility
    {
        public IList<string> GetLinesFromText(string body, int maxLines, bool doStrip = true)
        {
            IList<string> lines = new List<string>();
            using (StringReader sr = new StringReader(body))
            {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    if(lines.Count == 0 && line == "" && doStrip == true)
                    {
                        continue;
                    }

                    lines.Add(line);
                    if (lines.Count >= maxLines)
                    {
                        break;
                    }
                }
            }

            int i = lines.Count - 1;
            while (i >= 0)
            {
                if (lines[i] != "")
                {
                    break;
                }

                lines.RemoveAt(i);
                i--;
            }

            return lines;
        }
    }

    public class TempDirectory : IDisposable
    {
        string path;

        public TempDirectory()
        {
            path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
        }

        public void Dispose()
        {
            Directory.Delete(path, true);
        }

        public string TempPath
        {
            get
            {
                return path;
            }
        }
    }
}
