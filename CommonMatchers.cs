using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripPreamble
{
    public class CommonMatchers
    {
        public static string RX_SEPARATOR1 = @"^=+ *$";
        public static string RX_SEPARATOR2 = @"^\-+ *$";

        /*
        For detecting the SQL preamble that VS automatically adds:

            /*
            Post-Deployment Script Template							
            --------------------------------------------------------------------------------------
             This file contains SQL statements that will be appended to the build script.		
             Use SQLCMD syntax to include a file in the post-deployment script.			
             Example:      :r .\myfile.sql								
             Use SQLCMD syntax to reference a variable in the post-deployment script.		
             Example:      :setvar TableName MyTable							
                           SELECT * FROM [$(TableName)]					
            --------------------------------------------------------------------------------------
            * /
        */
        public static PreambleMatcher VisualStudioSql = new PreambleMatcher(
            new List<PreambleState>() {
                new PreambleState(new List<string>() { @"^/\*$" }, 1, 1),
                new PreambleState(new List<string>() { @"^(Post|Pre)\-Deployment Script Template$" }, 1, 1),
                new PreambleState(new List<string>() { RX_SEPARATOR2 }, 1, 1),
                new PreambleState(new List<string>() { @"^[^-]" }, 1, null),
                new PreambleState(new List<string>() { RX_SEPARATOR2 }, 1, 1),
                new PreambleState(new List<string>() { @"^\*/$" }, 1, 1),
                new PreambleState(new List<string>() { @"^$" }, 0, 1)
            }
        );

        /*
        Text with bars above and below:

            ====
            Text
            ====
        */
        public static PreambleMatcher SimpleBlock = new PreambleMatcher(
            new List<PreambleState>() {
                new PreambleState(new List<string>() { RX_SEPARATOR1, RX_SEPARATOR2 }, 1, 1),
                new PreambleState(new List<string>() { @"^[^=\-]" }, 1, null),
                new PreambleState(new List<string>() { RX_SEPARATOR1, RX_SEPARATOR2 }, 1, 1),
                new PreambleState(new List<string>() { @"^ *$" }, 0, 1)
            }
        );

        // No preamble will be matched. No lines will be consumed. Useful when 
        // we just want basic concatenation.
        public static PreambleMatcher NoPreamble = new PreambleMatcher(
            new List<PreambleState>() {
            }
        );

        public static IDictionary<string, PreambleMatcher> Matchers = new Dictionary<string, PreambleMatcher>()
        {
            { "vs_sql", VisualStudioSql },
            { "simple_block", SimpleBlock },
            { "none", NoPreamble }
        };
    }
}
