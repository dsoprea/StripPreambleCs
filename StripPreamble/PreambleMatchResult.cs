using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripPreamble
{
    public class PreambleMatchResult
    {
        bool matched;
        int rowsMatched;
        string context;

        public PreambleMatchResult(bool matched, int rowsMatched, string context = null)
        {
            this.matched = matched;
            this.rowsMatched = rowsMatched;
            this.context = context;
        }

        public bool Matched
        {
            get
            {
                return matched;
            }
        }

        public int RowsMatched
        {
            get
            {
                return rowsMatched;
            }
        }

        public string Context
        {
            get
            {
                return context;
            }
        }
    }
}
