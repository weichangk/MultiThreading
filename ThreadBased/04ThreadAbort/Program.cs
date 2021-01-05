using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _04ThreadAbort
{
    //终止线程
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting program...");
            Thread t = new Thread(PrintNumbersWithDelay);
            t.Start();
            Thread.Sleep(TimeSpan.FromSeconds(6));
            t.Abort();//强制终止线程；这很危险，不推荐使用
            Console.WriteLine("A thread has been aborted");
            Console.ReadLine();
        }

        static void PrintNumbersWithDelay()
        {
            Console.WriteLine("Starting...");
            for (int i = 1; i < 10; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                Console.WriteLine(i);
            }
        }
    }
}
