using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _05UseManualResetEventSlim
{
	/*
	使用ManualResetEventsSlim类来在线程间以更灵活的方式传递信号。
	ManualResetEvnetSlim的整个工作方式有点像人群通过大门。AutoResetEvent事件像一个旋转门,一次只允许一人通过。
	ManualResetEventSlim是ManualResetEvent的混合版本,一直保持大门敞开直到手动调用Reset方法。当调用_mainEvent.Set时,
	相当于打开了大门从而允许准备好的线程接收信号并继续工作。当调用mainEvent.Reset相当于关闭了大门。
	 */
	class Program
    {
        static void Main(string[] args)
        {
			var t1 = new Thread(() => TravelThroughGates("Thread 1", 5));
			var t2 = new Thread(() => TravelThroughGates("Thread 2", 6));
			var t3 = new Thread(() => TravelThroughGates("Thread 3", 12));
			t1.Start();
			t2.Start();
			t3.Start();
			Thread.Sleep(TimeSpan.FromSeconds(6));
			Console.WriteLine("The gates are now open!");
			_mainEvent.Set();
			Thread.Sleep(TimeSpan.FromSeconds(2));
			_mainEvent.Reset();
			Console.WriteLine("The gates have been closed!");
			Thread.Sleep(TimeSpan.FromSeconds(10));
			Console.WriteLine("The gates are now open for the second time!");
			_mainEvent.Set();
			Thread.Sleep(TimeSpan.FromSeconds(2));
			Console.WriteLine("The gates have been closed!");
			_mainEvent.Reset();
			Console.WriteLine("111111111111111111111");
			Console.ReadLine();
		}

		static void TravelThroughGates(string threadName, int seconds)
		{
			Console.WriteLine("{0} falls to sleep", threadName);
			Thread.Sleep(TimeSpan.FromSeconds(seconds));
			Console.WriteLine("{0} waits for the gates to open!", threadName);
			_mainEvent.Wait();
			Console.WriteLine("{0} enters the gates!", threadName);
		}

		static ManualResetEventSlim _mainEvent = new ManualResetEventSlim(false);
	}
}
