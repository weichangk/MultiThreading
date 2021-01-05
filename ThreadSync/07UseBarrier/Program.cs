using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _07UseBarrier
{
	/*
	Barrier类用于组织多个线程及时，在某个时刻碰面。其提供了一个回调函数,每次线程调用了SignalAndWait方法后该回调函数会被执行。
	创建了Barrier类,指定了我们想要同步两个线程。在两个线程中的任何最后一个调用了_barre.signalAndWait方法后,会执行一个回调函数来打印出阶段。
	每个线程将向Barrer发送两次信号,所以会有两个阶段。每个阶段的最后一个线程调用SignalAndWait方法时, Barrier将执行回调函数。
	这在多线程迭代运算中非常有用,可以在每个迭代结束前执行一些计算。当最后一个线程调用SignalAndWait方法时可以在迭代结束时进行交互。
	*/
	class Program
    {
        static void Main(string[] args)
        {
			var t1 = new Thread(() => PlayMusic("the guitarist", "play an amazing solo", 5));
			var t2 = new Thread(() => PlayMusic("the singer", "sing his song", 2));

			t1.Start();
			t2.Start();

			Console.ReadLine();
		}

		static Barrier _barrier = new Barrier(2, b => Console.WriteLine("End of phase {0}", b.CurrentPhaseNumber + 1));

		static void PlayMusic(string name, string message, int seconds)
		{
			for (int i = 1; i < 3; i++)
			{
				Console.WriteLine("----------------------------------------------");
				Thread.Sleep(TimeSpan.FromSeconds(seconds));
				Console.WriteLine("{0} starts to {1}", name, message);
				Thread.Sleep(TimeSpan.FromSeconds(seconds));
				Console.WriteLine("{0} finishes to {1}", name, message);
				_barrier.SignalAndWait();
			}
		}
	}
}
