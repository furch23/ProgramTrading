﻿using MrWangAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab8
{
    class Program
    {
        static MrWangConnection MrWangConnection;
        static bool Runing = true;
        static void Main(string[] args)
        {
            //初始化API元件
            init();
            //執行連線
            MrWangConnection.Connect("127.0.0.1", 5000);

            new Thread(() => { TimeOut(); }).Start();

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

                CanceledOrder canceledOrder = new CanceledOrder()
                {
                    Symbol = "TXFD9",
                    Side = SideEnum.Buy,
                    Qty = 1,
                    Price = 10800,
                    OrderType = OrderTypeEnum.otLimit,
                    TimeInForce = TimeInForceEnum.ROD,
                    TimeOut = DateTime.Now.AddSeconds(10)
                };

                SendTimeCancelOrder(canceledOrder);
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

        }

        /// <summary>
        /// 通知成交價
        /// </summary>
        private static void MrWangConnection_OnMatchInfo(Match match)
        {

        }

        static List<CanceledOrder> listTimeCancel = new List<CanceledOrder>();

        private static void SendTimeCancelOrder(CanceledOrder canceledOrder)
        {
            canceledOrder.OrderBook = MrWangConnection.SnedOrder(canceledOrder);
            listTimeCancel.Add(canceledOrder);
        }

        
        private static void TimeOut()
        {
            List<CanceledOrder> listRemove = new List<CanceledOrder>();

            while (Runing)
            {
                foreach (var order in listTimeCancel.ToArray())
                {
                    if (DateTime.Now > order.TimeOut)
                    {
                        MrWangConnection.CanceledOrder(order.OrderBook);
                        listRemove.Add(order);
                    }
                }

                if (listRemove.Count > 0)
                {
                    foreach (var order in listRemove)
                    {
                        listTimeCancel.Remove(order);
                    }

                    listRemove.Clear();
                }

                Thread.Sleep(1);
            }
        }
    }
}
