using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeingUI2
{
    public class OrderViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string _symbol;
        public string Symbol
        {
            get { return this._symbol; }

            set
            {
                if (value != this._symbol)
                {
                    this._symbol = value;
                    NotifyPropertyChanged("Symbol");
                }
            }
        }
        double _price;
        public double Price
        {
            get { return this._price; }

            set
            {
                if (value != this._price)
                {
                    this._price = value;
                    NotifyPropertyChanged("Price");
                }
            }
        }
        int _qty;
        public int Qty
        {
            get { return this._qty; }

            set
            {
                if (value != this._qty)
                {
                    this._qty = value;
                    NotifyPropertyChanged("Qty");
                }
            }
        }
        string _side;
        public string Side
        {
            get { return this._side; }

            set
            {
                if (value != this._side)
                {
                    this._side = value;
                    NotifyPropertyChanged("Side");
                }
            }
        }
        string _ordertype;
        public string OrderType
        {
            get { return this._ordertype; }

            set
            {
                if (value != this._ordertype)
                {
                    this._ordertype = value;
                    NotifyPropertyChanged("OrderType");
                }
            }
        }
        string _timeinforce;
        public string TimeInForce
        {
            get { return this._timeinforce; }

            set
            {
                if (value != this._timeinforce)
                {
                    this._timeinforce = value;
                    NotifyPropertyChanged("TimeInForce");
                }
            }
        }
        string _status;
        public string Status
        {
            get { return this._status; }

            set
            {
                if (value != this._status)
                {
                    this._status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        string _orderbook;
        public string OrderBook
        {
            get { return this._orderbook; }

            set
            {
                if (value != this._orderbook)
                {
                    this._orderbook = value;
                    NotifyPropertyChanged("OrderBook");
                }
            }
        }
    }
}
