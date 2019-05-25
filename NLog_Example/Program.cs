using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
namespace NLog_Example
{
    class Program
    {

        static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {

            logger.Info("Holle");
            logger.Error("1");

            
            C();
            B();
            A();
        }

        static void A()
        {
            logger.Error("2");
        }
        static void C()
        {
            logger.Error("3");
        }
        static void B()
        {
            logger.Error("5");
        }
    }
}
