using MrWangAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeingUI3
{
    class API
    {
        MrWangConnection MrWangConnection;
        Dictionary<string, Product> dicProduct;
        MainViewModel MainViewModel;
        public API(Dictionary<string, Product> dicProduct, MainViewModel MainViewModel)
        {
            MainViewModel.Log("API啟動!!");
            this.dicProduct = dicProduct;
            this.MainViewModel = MainViewModel;

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
                    MrWangConnection.SubscribeQuote(product.Key, 20);
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

        List<Reply> listReplyBuy = new List<Reply>();
        List<Reply> listReplySell = new List<Reply>();
        ConcurrentDictionary<string, OrderViewModel> dicOrderView = new ConcurrentDictionary<string, OrderViewModel>();
        /// <summary>
        /// 通知交易回報
        /// </summary>
        private void MrWangConnection_OnTradingReply(Reply reply)
        {
            OrderViewModel orderview;
            if (dicOrderView.ContainsKey(reply.OrderBook))
            {
                orderview = dicOrderView[reply.OrderBook];
            }
            else
            {
                orderview = new OrderViewModel()
                {
                    Symbol = reply.Symbol,
                    Price = reply.Price,
                    Qty = reply.Qty,
                    Side = reply.Side.ToString(),
                    OrderBook = reply.OrderBook,
                    OrderType = reply.orderType.ToString(),
                    TimeInForce = reply.TimeInForce.ToString(),
                    Status = reply.OrderStatus.ToString()
                };

                lock (MainViewModel.Orders) { MainViewModel.Orders.Insert(0, orderview); }

                MainViewModel.Orders.Insert(0, orderview);
                dicOrderView.TryAdd(reply.OrderBook, orderview);
            }

            orderview.Status = reply.OrderStatus.ToString();

            lock (listReplyBuy)
            {
                if (reply.OrderStatus == OrderStatusEnum.Filled)
                {
                    if (reply.Side == SideEnum.Buy)
                        listReplyBuy.Add(reply);
                    else
                        listReplySell.Add(reply);
                }

                if (listReplyBuy.Count > 0 && listReplyBuy.Count == listReplySell.Count)
                {
                    double Totoal = 0;
                    double Tax = 0;
                    for (int i = 0; i < listReplyBuy.Count; i++)
                    {
                        var Sell = listReplySell[i];
                        var Buy = listReplyBuy[i];

                        Totoal += Sell.Price - Buy.Price;

                        //交易稅
                        Tax += Math.Round(Sell.Price * Sell.Qty * 200 * 0.00002, 0);
                        Tax += Math.Round(Buy.Price * Buy.Qty * 200 * 0.00002, 0);
                        //手續費
                        Tax += Sell.Qty * 50;
                        Tax += Buy.Qty * 50;
                    }

                    MainViewModel.Tax = Tax;
                    MainViewModel.Total = Totoal;
                    MainViewModel.Amount = Totoal * 200 - Tax;
                }
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

        private static List<double> listAVG10Price = new List<double>();
        private static List<double> listAVG20Price = new List<double>();
        private static List<double> listAVG30Price = new List<double>();
        private static int Position = 0;
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

            if (match.Symbol == "TXFD9")
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

                if (AVG10Price > AVG20Price && AVG20Price > AVG30Price)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Order order = new Order()
                    {
                        Symbol = "TXFD9",
                        Side = SideEnum.Buy,
                        Qty = 1,
                        Price = match.MatchPrice,
                        OrderType = OrderTypeEnum.otMarket,
                        TimeInForce = TimeInForceEnum.IOC,
                    };

                    if (Position <= 0)
                    {
                        Position++;
                        MrWangConnection.SnedOrder(order);
                    }


                }
                else if (AVG10Price < AVG20Price && AVG20Price < AVG30Price)
                {
                    Console.ForegroundColor = ConsoleColor.Green;

                    Order order = new Order()
                    {
                        Symbol = "TXFD9",
                        Side = SideEnum.Sell,
                        Qty = 1,
                        Price = match.MatchPrice,
                        OrderType = OrderTypeEnum.otMarket,
                        TimeInForce = TimeInForceEnum.IOC,
                    };

                    if (Position >= 0)
                    {
                        Position--;
                        MrWangConnection.SnedOrder(order);
                    }

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}

