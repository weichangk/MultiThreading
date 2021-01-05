using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02QueueUserWorkItem
{
    //向线程池中放入异步操作
    class Program
    {
        static void Main(string[] args)
        {
            const int x = 1;
            const int y = 2;
            const string lambdaState = "lambda state 2";

            ThreadPool.QueueUserWorkItem(AsyncOperation);
            //Thread.Sleep(TimeSpan.FromSeconds(1));

            ThreadPool.QueueUserWorkItem(AsyncOperation, "async state");
            //Thread.Sleep(TimeSpan.FromSeconds(1));

            ThreadPool.QueueUserWorkItem(state =>
            {
                Console.WriteLine("Operation state: {0}", state);
                Console.WriteLine("Worker thread id: {0}", Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(TimeSpan.FromSeconds(2));
                Console.WriteLine("Worker thread id: {0} End............", Thread.CurrentThread.ManagedThreadId);
            }, "lambda state");// "lambda state"指的是state参数的默认值

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Console.WriteLine("Operation state: {0}, {1}", x + y, lambdaState);
                Console.WriteLine("Worker thread id: {0}", Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(TimeSpan.FromSeconds(3));
                Console.WriteLine("Worker thread id: {0} End............", Thread.CurrentThread.ManagedThreadId);
            }//, "lambda state"
           );
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("......");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            //Thread.Sleep(TimeSpan.FromSeconds(2));
            Console.ReadLine();
        }

        private static void AsyncOperation(object state)//state线程池中线程参数，可传可不传
        {
            Console.WriteLine("Operation state: {0}", state ?? "(null)");//空合并运算符(??)用于定义可空类型和引用类型的默认值;a??b 当a为null时则返回b，a不为null时则返回a本身。
            Console.WriteLine("Worker thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Console.WriteLine("Worker thread id: {0} End............", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
