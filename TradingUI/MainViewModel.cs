using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Data;

namespace TradingUI
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
            _logs = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(_logs, new object());
            new Thread(ProcessLog).Start();

            Log("成式啟動!!");

            dicProduct = new Dictionary<string, Product>();
            Products = new ObservableCollection<Product>();

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

            API = new API(dicProduct);
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
