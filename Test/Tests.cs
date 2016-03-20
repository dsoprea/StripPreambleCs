using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripPreamble
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestMatch()
        {
            string body = @"
=========
line2
line3
---------

hello
";

            // We create a new IList because the original can't be modified.
            Utility u = new Utility();
            IList<string> lines = u.GetLinesFromText(body, 10);
            CommonMatchers.SimpleBlock.TryStrip(lines);

            string actualOutput = String.Join(Environment.NewLine, lines);
            string expectedOutput = "hello";

            Assert.AreEqual(actualOutput, expectedOutput);
        }

        [TestMethod]
        public void TestConcatenate()
        {
            using(TempDirectory td = new TempDirectory())
            {
                string filename1 = "file1";
                string filePath1 = Path.Combine(td.TempPath, filename1);

                string filename2 = "file2";
                string filePath2 = Path.Combine(td.TempPath, filename2);

                string fileData1 = "aa";
                File.WriteAllText(filePath1, fileData1);

                string fileData2 = "bb";
                File.WriteAllText(filePath2, fileData2);

                Concatenate c = new Concatenate();
                IList<string> lines = c.Run(CommonMatchers.NoPreamble, new List<string>() { filePath1, filePath2 }, "=> ");

                string actualOutput = String.Join(Environment.NewLine, lines);
                string expectedOutputTemplate = @"=> {0}


{1}


=> {2}


{3}

";

                string expectedOutput = String.Format(expectedOutputTemplate, filePath1, fileData1, filePath2, fileData2);

                Assert.AreEqual(actualOutput, expectedOutput);
            }
        }

        [TestMethod]
        public void TestStripSqlPreambleAndConcatenate()
        {
            string filePath1 = "..\\..\\Test\\SqlFile1.sql";
            string filePath2 = "..\\..\\Test\\SqlFile2.sql";

            Concatenate c = new Concatenate();

            PreambleMatcher pm = CommonMatchers.VisualStudioSql;
            pm.Debug = true;
            IList<string> lines = c.Run(pm, new List<string>() { filePath1, filePath2 }, "=> ");

            string actualOutput = String.Join(Environment.NewLine, lines);
            string expectedOutputTemplate = @"=> {0}


PRE DEPLOY SQL CONTENT 2


=> {1}


POST DEPLOY SQL CONTENT 2


";

            string expectedOutput = String.Format(expectedOutputTemplate, filePath1, filePath2);

            Assert.AreEqual(actualOutput, expectedOutput);
        }
    }
}
