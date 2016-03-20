using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StripPreamble
{
    // Determine if our set of 
    public class PreambleMatcher
    {
        IList<PreambleState> states;
        bool isDebug = false;
        bool doStripLines = true;

        public PreambleMatcher(IList<PreambleState> states = null)
        {
            if (states != null)
            {
                this.states = states;
            }
            else {
                this.states = new List<PreambleState>();
            }
        }

        public PreambleMatchResult TryStrip(IList<string> lines)
        {
            int rowsMatched = 0;
            int i = 0;
            foreach (PreambleState state in states)
            {
                PreambleMatchResult pmr = state.Match(lines, stripLines: doStripLines);
                if (pmr.Matched)
                {
                    if (isDebug == true)
                    {
                        System.Diagnostics.Debug.WriteLine("HIT on state ({0}) [{1}].", i, state);
                    }

                    rowsMatched += pmr.RowsMatched;
                }
                else
                {
                    if (isDebug == true)
                    {
                        System.Diagnostics.Debug.WriteLine("MISS on state ({0}) [{1}]: [{2}]", i, state, (lines.Count() > 0 ? lines[0] : "<EOF>"));
                    }

                    return pmr;
                }

                i++;
            }

            return new PreambleMatchResult(true, rowsMatched);
        }

        public override string ToString()
        {
            return String.Format("PREAMBLE-MATCHER<({0})>", states.Count());
        }

        public bool Debug
        {
            set
            {
                isDebug = value;
            }
        }

        public bool StripLines
        {
            set
            {
                doStripLines = value;
            }
        }
    }
}
