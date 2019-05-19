using MrWangAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab8
{
    public class CanceledOrder : Order
    {
        public DateTime TimeOut { get; set; }
        public string OrderBook { get; set; }
    }
}
