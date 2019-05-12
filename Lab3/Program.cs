using MrWangAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    class Program
    {
        static MrWangConnection MrWangConnection;
        static void Main(string[] args)
        {
            //初始化API元件
            init();
            //執行連線
            MrWangConnection.Connect("127.0.0.1", 5000);

            while (true)
            {
                Console.ReadKey();
            }
        }

        private static void init()
        {
            //建立API執行個體
            MrWangConnection = new MrWangConnection(@"..\..\..\Data");
            //註冊連線事件
            MrWangConnection.OnConnected
                += new IMrWangConnectionEvents_OnConnectedEventHandler(MrWangConnection_OnConnected);
            //註冊斷線事件
            MrWangConnection.OnDisconnected
                += new IMrWangConnectionEvents_OnDisconnectedEventHandler(MrWangConnection_OnDisconnected);
            //註冊登入結果事件
            MrWangConnection.OnLogonReply
                += new IMrWangConnectionEvents_OnLogonReplyEventHandler(MrWangConnection_OnLogonReply);
            //註冊錯誤訊息事件
            MrWangConnection.OnErrorReply
                += new IMrWangConnectionEvents_OnErrorReplyEventHandler(MrWangConnection_OnErrorReply);
            //註冊回報事件
            MrWangConnection.OnTradingReply
                += new IMrWangConnectionEvents_OnTradingReplyEventHandler(MrWangConnection_OnTradingReply);
            //註冊行情報價事件
            MrWangConnection.OnOrderBookData
                += new IMrWangConnectionEvents_OnOrderBookDataEventHandler(MrWangConnection_OnOrderBookData);
            //註冊成交價事件
            MrWangConnection.OnMatchInfo
                += new IMrWangConnectionEvents_OnMatchInfoEventHandler(MrWangConnection_OnMatchInfo);
        }

        /// <summary>
        /// 通知連線成功事件
        /// </summary>
        private static void MrWangConnection_OnConnected()
        {
            Console.WriteLine("連線成功。");

            MrWangConnection.LogIn("A123456789", "1234");
        }

        /// <summary>
        /// 通知連線斷線事件
        /// </summary>
        private static void MrWangConnection_OnDisconnected()
        {
            Console.WriteLine("連線斷線。");
        }

        /// <summary>
        /// 通知登入結果事件
        /// </summary>
        private static void MrWangConnection_OnLogonReply(int Code, string Msg)
        {
            if (Code == 0)
            {
                //登入成功
                Console.WriteLine("登入成功。");
                Console.WriteLine("訂閱商品TXFD9。");
                MrWangConnection.SubscribeQuote("TXFD9", 20);
            }
            else
            {
                //登入失敗
                Console.WriteLine($"Code:{Code} Msg:{Msg}");
            }
        }

        /// <summary>
        /// 通知錯誤訊息事件
        /// </summary>
        private static void MrWangConnection_OnErrorReply(int Code, string Msg)
        {

        }

        /// <summary>
        /// 通知交易回報
        /// </summary>
        private static void MrWangConnection_OnTradingReply(Reply reply)
        {

        }

        /// <summary>
        /// 通知買賣報價
        /// </summary>
        private static void MrWangConnection_OnOrderBookData(OrderBook orderBook)
        {

        }

        private static List<double> listAVG10Price = new List<double>();
        private static List<double> listAVG20Price = new List<double>();
        private static List<double> listAVG30Price = new List<double>();

        /// <summary>
        /// 通知成交價
        /// </summary>
        private static void MrWangConnection_OnMatchInfo(Match match)
        {
            listAVG10Price.Add(match.MatchPrice);
            listAVG20Price.Add(match.MatchPrice);
            listAVG30Price.Add(match.MatchPrice);

            if (listAVG10Price.Count > 10) { listAVG10Price.RemoveAt(0); }
            if (listAVG20Price.Count > 30) { listAVG20Price.RemoveAt(0); }
            if (listAVG30Price.Count > 50) { listAVG30Price.RemoveAt(0); }

            double AVG10Price = Math.Round(listAVG10Price.Average(x => x), 0);
            double AVG20Price = Math.Round(listAVG20Price.Average(x => x), 0);
            double AVG30Price = Math.Round(listAVG30Price.Average(x => x), 0);

            Console.WriteLine($"{match.Time} -> Symbol:{match.Symbol}" +
            $" Last:{match.MatchPrice} x {match.MatchQty}" +
            $" Volume:{match.Volume} AVG10:{AVG10Price} AVG20:{AVG20Price} AVG30:{AVG30Price}");
        }
        //private static void MrWangConnection_OnMatchInfo(Match match)
        //{
        //    listAVG10Price.Add(match.MatchPrice);
        //    listAVG20Price.Add(match.MatchPrice);
        //    listAVG30Price.Add(match.MatchPrice);

        //    if (listAVG10Price.Count > 10) { listAVG10Price.RemoveAt(0); }
        //    if (listAVG20Price.Count > 30) { listAVG20Price.RemoveAt(0); }
        //    if (listAVG30Price.Count > 50) { listAVG30Price.RemoveAt(0); }

        //    double AVG10Price = Math.Round(listAVG10Price.Average(x => x), 0);
        //    double AVG20Price = Math.Round(listAVG20Price.Average(x => x), 0);
        //    double AVG30Price = Math.Round(listAVG30Price.Average(x => x), 0);

        //    if (AVG10Price > AVG20Price && AVG20Price > AVG30Price)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //    }
        //    else if (AVG10Price < AVG20Price && AVG20Price < AVG30Price)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Green;
        //    }
        //    else
        //    {
        //        Console.ForegroundColor = ConsoleColor.White;
        //    }

        //    Console.WriteLine($"{match.Time} -> Symbol:{match.Symbol}" +
        //    $" Last:{match.MatchPrice} x {match.MatchQty}" +
        //    $" Volume:{match.Volume} AVG10:{AVG10Price} AVG20:{AVG20Price} AVG30:{AVG30Price}");
        //}
    }
}

