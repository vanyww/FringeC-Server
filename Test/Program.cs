using MiddlewareLogic.Router;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TypeCreator;
using TypeCreator.Base;
using TypeCreator.Enums;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new RouteWorker();
            a.Start();
            
            while (true) Thread.Sleep(1000);
        }
    }
}
