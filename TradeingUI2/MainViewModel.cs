using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TradeingUI2
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static ConcurrentQueue<string> LogQueue = new ConcurrentQueue<string>();
        private static Logger NLog = LogManager.GetCurrentClassLogger();
        private API API;
        private Dictionary<string, Product> dicProduct;
        private bool Runing = true;
        public MainViewModel()
        {
            Logs = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(Logs, new object());
            new Thread(ProcessLog).Start();

            Log("成式啟動!!");

            dicProduct = new Dictionary<string, Product>();
            Products = new ObservableCollection<Product>();
            Orders = new ObservableCollection<OrderViewModel>();
            BindingOperations.EnableCollectionSynchronization(Orders, new object());

            Product product = new Product();
            product.Symbol = "TXFD9";
            product.SymbolColor = System.Windows.Media.Brushes.Red;

            Product product2 = new Product();
            product2.Symbol = "MXFD9";
            product2.SymbolColor = System.Windows.Media.Brushes.Black;

            dicProduct.Add("TXFD9", product);
            dicProduct.Add("MXFD9", product2);

            Products.Add(product);
            Products.Add(product2);

            Log("商品清單處理完畢!!");

            API = new API(dicProduct, Orders);
            Log("初始化完成。");
        }



        ObservableCollection<Product> _products;

        public ObservableCollection<Product> Products
        {
            get { return this._products; }

            set
            {
                if (value != this._products)
                {
                    this._products = value;
                    NotifyPropertyChanged("Products");
                }
            }
        }
        ObservableCollection<OrderViewModel> _orders;

        public ObservableCollection<OrderViewModel> Orders
        {
            get { return this._orders; }

            set
            {
                if (value != this._orders)
                {
                    this._orders = value;
                    NotifyPropertyChanged("Orders");
                }
            }
        }

        ObservableCollection<string> _logs;

        public ObservableCollection<string> Logs
        {
            get { return this._logs; }

            set
            {
                if (value != this._logs)
                {
                    this._logs = value;
                    NotifyPropertyChanged("Logs");
                }
            }
        }

        public void Close()
        {
            API.Close();
            Runing = false;
        }

        private void ProcessLog()
        {
            while (Runing)
            {
                if (LogQueue.Count > 0)
                {
                    LogQueue.TryDequeue(out string messenge);
                    NLog.Info(messenge);
                    Logs.Add(messenge);
                }
                Thread.Sleep(100);
            }
        }

        public void SendOrder(string Symbol, string Side, int Qty, double Price, string OrderType, string TimeInForce)
        {
            var orderview = new OrderViewModel()
            {
                Symbol = Symbol,
                Qty = Qty,
                Price = Price,
                Status = "等待委託",
                Side = Side,
                OrderType = OrderType,
                TimeInForce = TimeInForce
            };

            Orders.Add(orderview);

            MrWangAPI.TimeInForceEnum timeInForceEnum = MrWangAPI.TimeInForceEnum.ROD;
            MrWangAPI.OrderTypeEnum orderTypeEnum = MrWangAPI.OrderTypeEnum.otLimit;

            if (TimeInForce == "IOC")
                timeInForceEnum = MrWangAPI.TimeInForceEnum.IOC;
            else if (TimeInForce == "ROD")
                timeInForceEnum = MrWangAPI.TimeInForceEnum.ROD;

            if (OrderType == "LMT")
                orderTypeEnum = MrWangAPI.OrderTypeEnum.otLimit;
            else
                orderTypeEnum = MrWangAPI.OrderTypeEnum.otMarket;

            var order = new MrWangAPI.Order()
            {
                Symbol = Symbol,
                Qty = Qty,
                Price = Price,
                TimeInForce = timeInForceEnum,
                OrderType = orderTypeEnum
            };

            orderview.OrderBook = API.SendOrder(order);
        }

        public static void Log(string messenge)
        {
            LogQueue.Enqueue(messenge);
        }

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
