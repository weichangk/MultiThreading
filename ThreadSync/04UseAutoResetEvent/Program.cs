using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _04UseAutoResetEvent
{
    //AutoResetEvent类来从一个线程向另一个线程发送通知。AutoResetEvent类可以通知等待的线程有某事件发生。
    class Program
    {
        static void Main(string[] args)
        {
			var t = new Thread(() => Process(10));
			t.Start();

			Console.WriteLine("Waiting for another thread to complete work");
			_workerEvent.WaitOne();
			Console.WriteLine("First operation is completed!");
			Console.WriteLine("Performing an operation on a main thread");
			Thread.Sleep(TimeSpan.FromSeconds(5));
			_mainEvent.Set();
			Console.WriteLine("Now running the second operation on a second thread");
			_workerEvent.WaitOne();
			Console.WriteLine("Second operation is completed!");

			Console.ReadLine();
		}

		private static AutoResetEvent _workerEvent = new AutoResetEvent(false);
		private static AutoResetEvent _mainEvent = new AutoResetEvent(false);

		static void Process(int seconds)
		{
			Console.WriteLine("Starting a long running work...");
			Thread.Sleep(TimeSpan.FromSeconds(seconds));
			Console.WriteLine("Work is done!");
			_workerEvent.Set();
			Console.WriteLine("Waiting for a main thread to complete its work");
			_mainEvent.WaitOne();
			Console.WriteLine("Starting second operation...");
			Thread.Sleep(TimeSpan.FromSeconds(seconds));
			Console.WriteLine("Work is done!");
			_workerEvent.Set();
			Console.WriteLine("...........");
		}
	}
	/*
	 当主程序启动时,定义了两个AutoResetEvent实例。其中一个是从子线程向主线程发信号,另一个实例是从主线程向子线程发信号。
	 我们向AutoResetEvent构造方法传入false,定义了这两个实例的初始状态为unsignaled,这意味着任何线程调用这两个对象中的任何一个的WaitOne方法将会被阻塞,直到我们调用了Set方法。
	 如果初始事件状态为true,那么AutoResetEvent实例的状态为signaled,如果线程调用WaitOne方法则会被立即处理。
	 然后事件状态自动变为unsignaled,所以需要再对该实例调用一次Set方法,以便让其他的线程对该实例调用WaitOne方法从而继续执行。
	 AutoResetEvent类采用的是内核时间模式,所以等待时间不能太长。使用ManualResetEventslim类更好,因为它使用的是混合模式。
	 */
}
