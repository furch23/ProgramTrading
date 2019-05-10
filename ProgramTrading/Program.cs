using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrWangAPI;


namespace ProgramTrading
{
    class Program
    {
        static MrWangConnection MrWangConnection;
        static void Main(string[] args)
        {

            //第一步.初始化API元件
            init();

            //第二步.執行連線
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

            //第三步. 執行登入
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

                //第四步 訂閱報價
                MrWangConnection.SubscribeQuote("TXFD9");
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
            Console.WriteLine($"Status:{reply.OrderStatus} {reply.Side} " +
                              $"{reply.Qty} {reply.Symbol} @ {reply.Price} " +
                              $"{reply.orderType} {reply.TimeInForce}");
        }

        /// <summary>
        /// 通知買賣報價
        /// </summary>
        private static void MrWangConnection_OnOrderBookData(OrderBook orderBook)
        {
            Console.WriteLine($"Symbol:{orderBook.Symbol}" +
                $" Bid:{orderBook.BidPrice} x {orderBook.BidQty}" +
                $" Ask:{orderBook.AskPrice} x {orderBook.AskQty}");
        }

        /// <summary>
        /// 通知成交價
        /// </summary>
        private static void MrWangConnection_OnMatchInfo(Match match)
        {
            Console.WriteLine($"Symbol:{match.Symbol}" +
                $" Last:{match.MatchPrice} x {match.MatchQty}" +
                $" Volume:{match.Volume}");
        }
    }
}
