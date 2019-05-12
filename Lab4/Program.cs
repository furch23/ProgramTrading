using MrWangAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
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

        static Dictionary<string, KLine> dicKLine = new Dictionary<string, KLine>();
        static string LastTime = null;
        /// <summary>
        /// 通知成交價
        /// </summary>
        private static void MrWangConnection_OnMatchInfo(Match match)
        {
            var time = match.Time.Substring(0, 5) + ":00";

            KLine kline;

            if (dicKLine.ContainsKey(time))
            {
                kline = dicKLine[time];

                if (kline.High < match.MatchPrice)
                    kline.High = match.MatchPrice;

                if (kline.Low > match.MatchPrice)
                    kline.Low = match.MatchPrice;

                kline.Volume += match.MatchQty;

                Console.ForegroundColor = ConsoleColor.Green;

                Console.Write("\rTime:{0} Open:{1} High:{2} Low:{3} Close:{4} Volume:{5} \b"
                , match.Time, kline.Open, kline.High, kline.Low, kline.Close, kline.Volume);


            }
            else
            {
                kline = new KLine();
                dicKLine.Add(time, kline);
                kline.Open = match.MatchPrice;
                kline.High = match.MatchPrice;
                kline.Low = match.MatchPrice;
                kline.Close = match.MatchPrice;
                kline.Volume = match.MatchQty;

                if (LastTime != null)
                {
                    Console.Write("\r");

                    var lastkline = dicKLine[LastTime];

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Time:{0} Open:{1} High:{2} Low:{3} Close:{4} Volume:{5}"
                    , time, lastkline.Open, lastkline.High, lastkline.Low, lastkline.Close, lastkline.Volume);
                }
                Console.ForegroundColor = ConsoleColor.Green;

                Console.Write("\rTime:{0} Open:{1} High:{2} Low:{3} Close:{4} Volume:{5} \b"
                , match.Time, kline.Open, kline.High, kline.Low, kline.Close, kline.Volume);

            }


            LastTime = time;

            //if (type == 1)
            //{
            //    Console.ForegroundColor = ConsoleColor.White;
            //    if (LiveFlag)
            //    {
            //        Console.Write("\r");
            //    }

            //    Console.Write("MarketNo:{0} Index:{1} Date:{2} Time:{3} Open:{4} High:{5} Low:{6} Close:{7} Qty:{8} \n"
            //    , marketNo, index, date, time.ToString().PadLeft(6, '0'), openPrice, highPrice, lowPrice, closePrice, qty);
            //    LiveFlag = false;
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    LiveFlag = true;
            //    Console.Write("\rMarketNo:{0} Index:{1} Date:{2} Time:{3} Open:{4} High:{5} Low:{6} Close:{7} Qty:{8} \b"
            //   , marketNo, index, date, time.ToString().PadLeft(6, '0'), openPrice, highPrice, lowPrice, closePrice, qty);
            //}
        }
    }
}
