using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeingUI3
{
    public class Product : INotifyPropertyChanged
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

        private System.Windows.Media.Brush _symbolcolor;
        public System.Windows.Media.Brush SymbolColor
        {
            get
            {
                return this._symbolcolor;
            }

            set
            {
                if (value != this._symbolcolor)
                {
                    this._symbolcolor = value;
                    NotifyPropertyChanged("SymbolColor");
                }
            }
        }

        double _last;
        public double Last
        {
            get
            {
                return this._last;
            }

            set
            {
                if (value != this._last)
                {
                    this._last = value;
                    NotifyPropertyChanged("Last");
                    if (_last > _reference)
                        LastColor = System.Windows.Media.Brushes.Red;
                    else if (_last < _reference)
                        LastColor = System.Windows.Media.Brushes.Green;
                    else
                        LastColor = System.Windows.Media.Brushes.White;
                }
            }
        }
        private System.Windows.Media.Brush _lastcolor;
        public System.Windows.Media.Brush LastColor
        {
            get
            {
                return this._lastcolor;
            }

            set
            {
                if (value != this._lastcolor)
                {
                    this._lastcolor = value;
                    NotifyPropertyChanged("LastColor");
                }
            }
        }

        int _lastqty;
        public int LastQty
        {
            get
            {
                return this._lastqty;
            }

            set
            {
                if (value != this._lastqty)
                {
                    this._lastqty = value;
                    NotifyPropertyChanged("LastQty");
                }
            }
        }

        double _bid;
        public double Bid
        {
            get
            {
                return this._bid;
            }

            set
            {
                if (value != this._bid)
                {
                    this._bid = value;
                    NotifyPropertyChanged("Bid");
                    if (_bid > _reference)
                        BidColor = System.Windows.Media.Brushes.Red;
                    else if (_bid < _reference)
                        BidColor = System.Windows.Media.Brushes.Green;
                    else
                        BidColor = System.Windows.Media.Brushes.White;
                }
            }
        }
        private System.Windows.Media.Brush _bidcolor;
        public System.Windows.Media.Brush BidColor
        {
            get
            {
                return this._bidcolor;
            }

            set
            {
                if (value != this._bidcolor)
                {
                    this._bidcolor = value;
                    NotifyPropertyChanged("BidColor");
                }
            }
        }
        int _bidqty;
        public int BidQty
        {
            get
            {
                return this._bidqty;
            }

            set
            {
                if (value != this._bidqty)
                {
                    this._bidqty = value;
                    NotifyPropertyChanged("BidQty");
                }
            }
        }
        double _ask;
        public double Ask
        {
            get
            {
                return this._ask;
            }

            set
            {
                if (value != this._ask)
                {
                    this._ask = value;
                    NotifyPropertyChanged("Ask");
                    if (_ask > _reference)
                        AskColor = System.Windows.Media.Brushes.Red;
                    else if (_ask < _reference)
                        AskColor = System.Windows.Media.Brushes.Green;
                    else
                        AskColor = System.Windows.Media.Brushes.White;
                }
            }
        }
        private System.Windows.Media.Brush _askcolor;
        public System.Windows.Media.Brush AskColor
        {
            get
            {
                return this._askcolor;
            }

            set
            {
                if (value != this._askcolor)
                {
                    this._askcolor = value;
                    NotifyPropertyChanged("AskColor");
                }
            }
        }
        int _askqty;
        public int AskQty
        {
            get
            {
                return this._askqty;
            }

            set
            {
                if (value != this._askqty)
                {
                    this._askqty = value;
                    NotifyPropertyChanged("AskQty");
                }
            }
        }

        int _volume;
        public int Volume
        {
            get
            {
                return this._volume;
            }

            set
            {
                if (value != this._volume)
                {
                    this._volume = value;
                    NotifyPropertyChanged("Volume");
                }
            }
        }

        double _reference;
        public double Reference
        {
            get
            {
                return this._reference;
            }

            set
            {
                if (value != this._reference)
                {
                    this._reference = value;
                    NotifyPropertyChanged("Reference");
                }
            }
        }
    }
}
