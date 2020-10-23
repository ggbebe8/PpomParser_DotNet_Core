using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace PpomppuParsing_Core
{
    class MainView
    {
        Play pl = new Play();
        Timer time;
        public void WelcomeView()
        {
            time = new Timer(60000);
            time.Elapsed += OnTimedEvent;
            time.Enabled = false;

            while(true)
            {
                Console.WriteLine("=====Welcome PPomParser=====");
                Console.WriteLine("What you're looking for now : ");
                for (int i = 0; i < pl.mFindGoods.Count; i++)
                {
                    Console.Write(pl.mFindGoods[i]);
                    if (i != pl.mFindGoods.Count - 1)
                        Console.Write(", ");
                }
                Console.WriteLine("\r\n===========================");
                Console.WriteLine("\r\n");
                Console.WriteLine("1. Add Goods");
                Console.WriteLine("2. Remove Goods");
                Console.WriteLine("3. Run");
                Console.WriteLine("9. Exit");
                Console.Write("\r\nINPUT : ");
                string sInput = Console.ReadLine();
                if (sInput == "1")
                {
                    Console.Write("Add Goods Input : ");
                    if (pl.AddGoods(Console.ReadLine().Trim()))
                    {
                        Console.WriteLine("Input Success");
                    }
                    else
                    {
                        Console.WriteLine("Input Fail. plz Input Enter");
                        Console.ReadLine();
                    }
                }

                else if (sInput == "2")
                {
                    Console.Write("Remove Goods Input : ");
                    if (pl.DelGoods(Console.ReadLine().Trim()))
                    {
                        Console.WriteLine("Remove Success");
                    }
                    else
                    {
                        Console.WriteLine("Remove Fail. plz Input Enter");
                        Console.ReadLine();
                    }
                }
                else if (sInput == "3")
                {
                    time.Enabled = true;
                    time.Start();
                    Console.WriteLine("Run....");
                    Console.WriteLine("If you want to stop, input enter");
                    Console.ReadLine();
                }
                else if (sInput == "9")
                {
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Wrong Input. Input Enter");
                    Console.ReadLine();
                }
                Console.Clear();
                
            }

        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            pl.fnAlarm();
        }
        
    }
}
