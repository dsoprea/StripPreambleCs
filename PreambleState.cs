using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StripPreamble
{
    // Describes a set of regular-expressions that might represent the same 
    // type of line.
    public class PreambleState
    {
        IList<Regex> lineMatchers;
        int? minLines;
        int? maxLines;

        public PreambleState(IList<string> lineMatcherPhrases, int? minLines, int? maxLines)
        {
            IList<Regex> lineMatchers = new List<Regex>();
            foreach(string matchPhrase in lineMatcherPhrases)
            {
                lineMatchers.Add(new Regex(matchPhrase));
            }

            this.lineMatchers = lineMatchers;

            this.minLines = minLines;
            this.maxLines = maxLines;
        }

        public PreambleMatchResult Match(IList<string> lines, bool stripLines = true)
        {
            int rowsMatched = 0;

            while (true)
            {
                // If we've exhausted the rows;
                if (lines.Count == 0)
                {
                    if ((minLines == null || rowsMatched >= minLines) && (maxLines == null || rowsMatched <= maxLines))
                    {
                        return new PreambleMatchResult(true, rowsMatched);
                    }
                    else
                    {
                        return new PreambleMatchResult(false, 0);
                    }
                }

                string currentLine = lines[0];
                lines.RemoveAt(0);

                if(stripLines == true)
                {
                    currentLine = currentLine.Trim();
                }

                bool matched = false;
                foreach (Regex rx in lineMatchers)
                {
                    if (rx.IsMatch(currentLine) == true)
                    {
                        matched = true;
                        break;
                    }
                }

                if (matched == true)
                {
                    rowsMatched += 1;
                    continue;
                }

                // No match. Put it back.
                lines.Insert(0, currentLine);

                // If we get here, we ran into a line that didn't match.
                if ((minLines == null || rowsMatched >= minLines) && (maxLines == null || rowsMatched <= maxLines))
                {
                    return new PreambleMatchResult(true, rowsMatched);
                }
                else
                {
                    return new PreambleMatchResult(false, 0, currentLine);
                }
            }
        }

        public override string ToString()
        {
            return String.Format("PREAMBLE-STATE<[{0}] [{1}] [{2}]", String.Join("|||", lineMatchers), minLines, maxLines);
        }
    }
}
