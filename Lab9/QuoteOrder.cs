using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab9
{
    public class QuoteOrder
    {
        public string Symbol { get; set; }
        public double Ask { get; set; }
        public int AskQty { get; set; }
        public double Bid { get; set; }
        public int BidQty { get; set; }
        public string AskOrderBook { get; set; }
        public string BidOrderBook { get; set; }        
    }
}
