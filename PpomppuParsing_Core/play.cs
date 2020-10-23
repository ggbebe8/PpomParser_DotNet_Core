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
        List<string> mGetInfoList = new List<string>();
        List<string> mChkInfoList = new List<string>();

        bool mExit = false;

        #endregion 전역변수_End

        #region 생성자
        public Play()
        {
            mURL = "http://www.ppomppu.co.kr/zboard/zboard.php?id=ppomppu&page=1&divpage=53";
            mStartStr = "<font class=list_title>";
            mLastStr = "</font>";
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
                StreamReader objReader = new StreamReader(@".\List.ps");
                strList = objReader.ReadLine().Trim();
                objReader.Close();

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
                StreamWriter objWriter = new StreamWriter(@".\List.ps", false);
                objWriter.Write(strSumList);
                objWriter.Close();
                return;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString(), "오 류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        //알림
        public void fnAlarm()
        {
            string strFindOK = "";

            mGetInfoList = Paser.GetInfo(mURL, mStartStr, mLastStr);
            if (mGetInfoList.Count < 1)
            {
                //MessageBox.Show("정보를 파싱할 수 없음", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            foreach (string wantToFind in mFindGoods)
            {

                for (int i = 0; i < mGetInfoList.Count; i++)
                {
                    if (mChkInfoList.Contains(mGetInfoList[i]))
                        continue;

                    else if (mGetInfoList[i].Contains(wantToFind.ToUpper()))    //무조건 대문자로 변환
                    {
                        strFindOK += mGetInfoList[i] + Environment.NewLine;
                        mChkInfoList.Add(mGetInfoList[i]);
                    }
                }
            }

            if (mChkInfoList.Count > 100)
            {
                mChkInfoList.RemoveRange(0, 50);
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