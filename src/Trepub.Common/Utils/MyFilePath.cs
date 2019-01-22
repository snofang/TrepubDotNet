using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Trepub.Common.Utils
{
    public class MyFilePath
    {
        //$@".\DataTemplates\{ust}Test_Template_simple.txt"
        public static string GetBin_Path()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        public static string GetBin_Upper_TwoLevel_Path()
        {
            return Path.Combine(GetBin_Path(), Path.Combine("..", ".."));
        }
        public static string GetRawCDRFileTemplate_Path(string File)
        {
            return Path.Combine(GetBin_Path(), Path.Combine("Billing", "DataTemplates", File));
        }
        public static string GetRawCDRFileFormatTemplate_Path(string File)
        {
            return Path.Combine(GetBin_Path(), Path.Combine("Billing", File));
        }
        public static string GetTestRawCDRFile_Path(string ust)
        {
            var p = Path.Combine(GetBin_Upper_TwoLevel_Path(), Path.Combine("Data", "Test", ust));
            Directory.CreateDirectory(p);
            return p;
        }
        public static string GetRawCDRFile_Path(string ust)
        {
            var p = Path.Combine(GetBin_Upper_TwoLevel_Path(), Path.Combine("Data", ust));
            Directory.CreateDirectory(p);
            return p;
        }
        public static string GetRawCDRDoneFile_Path(string ust, string fn)
        {
            var dtstr = DateTime.Now.ToString("yyyyMMdd");
            //NEGINTEL_TE_CCN17_H-VOIC_0331_20171228160215_M
            Regex r = new Regex(@"20\d{2}[01]\d[0123]\d");
            Match m = r.Match(fn);
            if (m.Success)
            {
                dtstr = m.Value;
            }
            var p = Path.Combine(GetBin_Upper_TwoLevel_Path(), Path.Combine("DataDone", ust, dtstr));
            Directory.CreateDirectory(p);
            return p;
        }
    }
}
