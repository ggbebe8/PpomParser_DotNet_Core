using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Net.Mail;

namespace PpomppuParsing_Core
{
    class Play
    {
        #region 전역변수

        string mURL = "";
        string mStartStr = "";
        string mLastStr = "";
        public List<string> mFindGoods = new List<string>();
       
        List<string> mChkInfoList = new List<string>();
        List<stSearched> mSearchedItem = new List<stSearched>();

        struct stSearched
        {
            public string sItem;
            public DateTime dtAddDate;
            public stSearched(string item, DateTime dateTime)
            {
                sItem = item;
                dtAddDate = dateTime;
            }
        }
        

        #endregion 전역변수_End

        #region 생성자
        public Play()
        {
            mURL = "http://www.ppomppu.co.kr/zboard/zboard.php?id=ppomppu&page=1&divpage=53";
            mStartStr = "<font class=list_title>";
            mLastStr = "</font>";
            Log.Info("Start");
            fnFileRead();
        }

        #endregion 생성자_End


        #region 함수
        //파일읽고 초기화
        private void fnFileRead()
        {

            try
            {
                string strList = "";
                using (StreamReader objReader = new StreamReader(@".\List.ps"))
                {
                    strList = objReader.ReadLine().Trim();
                    objReader.Close();
                }

                string[] tempItem = new string[] { };
                tempItem = strList.Split('|');

                foreach (string tmp in tempItem)
                {
                    mFindGoods.Add(tmp);
                }
                return;
            }
            catch
            {
                return;
            }
        }

        //파일쓰기
        private void fnFileWrite()
        {
            string strSumList = "";

            if (mFindGoods.Count > 0)
            {
                foreach (string temp in mFindGoods)
                {
                    strSumList += temp + "|";
                }

                strSumList = strSumList.Substring(0, strSumList.Length - 1);
            }

            try
            {
                
                if(!File.Exists(@".\List.ps"))
                {
                    File.Create(@".\List.ps");
                }

                using (StreamWriter objWriter = new StreamWriter(@".\List.ps", false))
                {
                    objWriter.Write(strSumList);
                    objWriter.Close();
                }

                return;
            }
            catch (Exception e)
            {
                Log.Info(string.Format("fnFileWrite() : {0}", e.ToString()));
                return;
            }
        }

        //알림
        public void fnAlarm()
        {
            List<string> GetInfoList = new List<string>();
            string strFindOK = "";

            GetInfoList = Paser.GetInfo(mURL, mStartStr, mLastStr);

            if (GetInfoList.Count < 1)
            {
                Log.Info(string.Format("fnAlarm() : 파싱된 정보가 없음"));
            }

            TimeSpan ts;
            for (int i = 0; i < mSearchedItem.Count; i++)
            {
                //검색된 리스트는 하루 지나면 삭제
                ts = DateTime.Now - mSearchedItem[i].dtAddDate;
                if (ts.Days > 0)
                {
                    mSearchedItem.RemoveAt(i);
                    i--;
                    continue;
                }

                //이미 검색된 거라면 보낼 메세지에서 제거
                for (int x = 0; x < GetInfoList.Count; x++)
                {
                    if (GetInfoList[x].Contains(mSearchedItem[i].sItem))
                    {
                        GetInfoList.RemoveAt(x);
                        x--;
                    }
                }


            } 

            foreach (string wantToFind in mFindGoods)
            {

                for (int i = 0; i < GetInfoList.Count; i++)
                {
                    if (GetInfoList[i].Contains(wantToFind.ToUpper()))    //무조건 대문자로 변환
                    {
                        strFindOK += GetInfoList[i] + Environment.NewLine;
                        stSearched searched = new stSearched(GetInfoList[i], DateTime.Now);
                        mSearchedItem.Add(searched);
                    }
                }
            }

            if (mSearchedItem.Count > 100)
            {
                mSearchedItem.RemoveRange(0, 50);
            }

            if (strFindOK != "")
            {
                Telegram_Bot.Telegram_Send(strFindOK);
                //fnMailSend(strFindOK);
            }
        }

        #endregion 함수_End
        
        //추가버튼
        public bool AddGoods(string p_goods)
        {
            if (p_goods.Trim().Replace(" ", "") == "")
            {
                //MessageBox.Show("빈 값 입력불가", "빈 값", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            mFindGoods.Add(p_goods);
            fnFileWrite();

            return true;
        }

        //삭제버튼
        public bool DelGoods(string p_goods)
        {
            if (p_goods.Trim().Replace(" ", "") == "")
            {
                //MessageBox.Show("삭제할 내용이 없음", "빈 값", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            else if (!mFindGoods.Contains(p_goods))
            {
                //MessageBox.Show("삭제할 이름이 옳바르지 않음", "빈 값", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false; 
            }
            else
            {
                mFindGoods.Remove(p_goods);
                fnFileWrite();
                return true;
            }
        }

        //내용확인버튼
        private void ViewCurrent()
        {
            //List<string> liInfo = Paser.GetInfo(mURL, mStartStr, mLastStr);
            List<string> liFindWhat = new List<string>();
            liFindWhat.Add(mStartStr + "|&|" + mLastStr);
            liFindWhat.Add("'eng list_vspace' colspan=2  title=" + "|&|" + ">");

            List<string> liInfo = Paser.GetHtmlInfo(mURL, liFindWhat);

            string strInfo = "";

            for (int i = 0; i < liInfo.Count; i++)
            {
                strInfo += liInfo[i] + Environment.NewLine + Environment.NewLine;
            }

            //MessageBox.Show(strInfo);
        }


    }
}