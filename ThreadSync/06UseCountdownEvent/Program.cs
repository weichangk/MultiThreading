using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _06UseCountdownEvent
{
	/*
	 * 使用CountdownEvent信号类来等待直到一定数量的操作完成。
	 * 当主程序启动时,创建了一个CountdownEvent实例,在其构造函数中指定了当两个操作完成时会发出信号。
	 * 然后我们启动了两个线程,当它们执行完成后会发出信号。一旦第二个线程完成,主线程会从等待CountdownEvent的状态中返回并继续执行。
	 * 针对需要等待多个异步操作完成的情形,使用该方式是非常便利的。
	 * 然而这有一个重大的缺点。如果调用_countdown.Signal()没达到指定的次数,那么_countdown. Wait)将一直等待。
	 * 请确保使用CountdownEvent时,所有线程完成后都要调用Signal方法。
	 */

	class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine("Starting two operations");
			var t1 = new Thread(() => PerformOperation("Operation 1 is completed", 4));
			var t2 = new Thread(() => PerformOperation("Operation 2 is completed", 8));
			t1.Start();
			t2.Start();
			_countdown.Wait();
			Console.WriteLine("Both operations have been completed.");
			_countdown.Dispose();
			Console.ReadLine();
		}

		static CountdownEvent _countdown = new CountdownEvent(2);

		static void PerformOperation(string message, int seconds)
		{
			Thread.Sleep(TimeSpan.FromSeconds(seconds));
			Console.WriteLine(message);
			_countdown.Signal();
		}
	}
}
