using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PpomppuParsing_Core
{
    class Log
    {
        public static void Info(string sLog)
        {
            string sFileName = "ParserLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            sLog = DateTime.Now.ToString("o") + " - " + sLog;

            string sFolderPath = ".\\Log";
            DirectoryInfo di = new DirectoryInfo(sFolderPath);

            if (!di.Exists)
                di.Create();

            using (StreamWriter sw = new StreamWriter(string.Format(".\\Log\\{0}", sFileName), true))
            {
                sw.WriteLine(sLog);
                sw.Close();
            }
        }
    }
}