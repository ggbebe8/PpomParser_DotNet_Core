using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace PpomppuParsing_Core
{
    class Paser
    {
        /// MainParser ///
        public static List<string> GetInfo(string p_URL, string p_StartStr, string p_LastStr)        //parsing
        {
            List<string> liWantToFind = new List<string>();

            String strURL = p_URL;//
            String strStartStr = p_StartStr;//
            String strLastStr = p_LastStr;//

            String strAllParsing = GetHtmlString(strURL);

            if (strAllParsing == "False")
                return liWantToFind;

            int intFindFirstIndex = 0;
            int intFindLastIndex = 0;
            string strTemp = "";



            while (true)
            {
                strTemp = "";

                //해당 인덱스를 찾는다. 
                intFindFirstIndex = strAllParsing.IndexOf(strStartStr, intFindFirstIndex) + strStartStr.Length;
                intFindLastIndex = strAllParsing.IndexOf(strLastStr, intFindFirstIndex);

                //더이상 못찾으면 나가기
                if (intFindFirstIndex - strStartStr.Length < 0 || intFindLastIndex < 0)
                    break;

                for (int i = intFindFirstIndex; i < intFindLastIndex; i++)
                {
                    strTemp += strAllParsing[i];
                }

                liWantToFind.Add(strTemp.ToUpper());
            }

            return liWantToFind;
        }

        // Parsing 
        // p_where : "StartValue|&|LastValue"
        public static List<string> GetHtmlInfo(string p_url, List<string> p_where)
        {

            List<string> liWantToFind = new List<string>();

            String strURL = p_url;//
            String strStartStr = "";//
            String strLastStr = "";//

            String strAllParsing = GetHtmlString(strURL);

            bool isWhileStop = false;

            if (strAllParsing == "False")
                return liWantToFind;

            int intFindFirstIndex = 0;
            int intFindLastIndex = 0;
            string strTemp = "";
            string[] strFull = new string[] { };


            while (!isWhileStop)
            {
                for (int i = 0; i < p_where.Count; i++)
                {
                    strFull = p_where[i].Split(new string[] { "|&|" }, StringSplitOptions.None);

                    if (strFull.Length != 2)
                    {
                        liWantToFind.Clear();
                        return liWantToFind;
                    }

                    strStartStr = strFull[0];
                    strLastStr = strFull[1];

                    //해당 인덱스를 찾는다. 
                    intFindFirstIndex = strAllParsing.IndexOf(strStartStr, intFindFirstIndex) + strStartStr.Length;
                    intFindLastIndex = strAllParsing.IndexOf(strLastStr, intFindFirstIndex);

                    //더이상 못찾으면 나가기
                    if (intFindFirstIndex - strStartStr.Length < 0 || intFindLastIndex < 0)
                    {
                        isWhileStop = true;
                        break;
                    }
                    for (int x = intFindFirstIndex; x < intFindLastIndex; x++)
                    {
                        strTemp += strAllParsing[x];
                    }
                    strTemp += "|,|";

                    liWantToFind.Add(strTemp.ToUpper());

                    strTemp = "";
                    intFindFirstIndex = intFindLastIndex + strStartStr.Length;

                }
            }

            return liWantToFind;


        }

        // GetHtml //
        private static String GetHtmlString(String url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                String strHtml = reader.ReadToEnd();
                reader.Close();
                response.Close();
                return strHtml;
            }
            catch (WebException)
            {
                return "False";
            }
        }
    }
}
