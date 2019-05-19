﻿using MrWangAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeingUI2
{
    class API
    {
        MrWangConnection MrWangConnection;
        Dictionary<string, Product> dicProduct;
        ObservableCollection<OrderViewModel> obsOrderViewModel;
        public API(Dictionary<string, Product> dicProduct, ObservableCollection<OrderViewModel> obsOrderViewModel)
        {
            MainViewModel.Log("API啟動!!");
            this.dicProduct = dicProduct;
            this.obsOrderViewModel = obsOrderViewModel;

            init();

            MrWangConnection.Connect("127.0.0.1", 5000);
            MainViewModel.Log("API啟動完畢!!");
        }

        private void init()
        {
            MainViewModel.Log("API執行初始化...");
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

        public void Close()
        {
            MrWangConnection.DisConnect();
        }

        public string SendOrder(Order order)
        {
            return MrWangConnection.SnedOrder(order);
        }

        /// <summary>
        /// 通知連線成功事件
        /// </summary>
        private void MrWangConnection_OnConnected()
        {
            Console.WriteLine("連線成功。");

            MrWangConnection.LogIn("A123456789", "1234");
        }

        /// <summary>
        /// 通知連線斷線事件
        /// </summary>
        private void MrWangConnection_OnDisconnected()
        {
            Console.WriteLine("連線斷線。");
        }

        /// <summary>
        /// 通知登入結果事件
        /// </summary>
        private void MrWangConnection_OnLogonReply(int Code, string Msg)
        {
            if (Code == 0)
            {
                //登入成功
                Console.WriteLine("登入成功。");
                Console.WriteLine("訂閱商品TXD9。");

                foreach (var product in dicProduct)
                {
                    MrWangConnection.SubscribeQuote(product.Key, 5);
                }
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
        private void MrWangConnection_OnErrorReply(int Code, string Msg)
        {

        }

        /// <summary>
        /// 通知交易回報
        /// </summary>
        private void MrWangConnection_OnTradingReply(Reply reply)
        {
            var orderview = obsOrderViewModel.FirstOrDefault(x=>x.OrderBook == reply.OrderBook);

            if (orderview != null)
            {
                orderview.Status = reply.OrderStatus.ToString();
            }
        }

        /// <summary>
        /// 通知買賣報價
        /// </summary>
        private void MrWangConnection_OnOrderBookData(OrderBook orderBook)
        {
            if (dicProduct.ContainsKey(orderBook.Symbol))
            {
                var product = dicProduct[orderBook.Symbol];
                product.Bid = orderBook.BidPrice;
                product.Ask = orderBook.AskPrice;
                product.BidQty = orderBook.BidQty;
                product.AskQty = orderBook.AskQty;
                product.Reference = orderBook.ReferencePrice;
            }
        }

        /// <summary>
        /// 通知成交價
        /// </summary>
        private void MrWangConnection_OnMatchInfo(Match match)
        {
            if (dicProduct.ContainsKey(match.Symbol))
            {
                var product = dicProduct[match.Symbol];
                product.Last = match.MatchPrice;
                product.LastQty = match.MatchQty;
                product.Volume = match.Volume;
            }
        }
    }
}

