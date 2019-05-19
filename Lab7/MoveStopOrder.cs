using MrWangAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab7
{
    class MoveStopOrder : Order
    {
        public double TickCount { get; set; }
        public double StopPrice { get; set; }
        public double Tick { get; set; }
    }
}
