using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01CallDelegate
{

	/*
	在线程池中调用委托
	事实上使用AsyncWaitHandle并不是必要的。如果注释掉rAsyncWaitHandle.WaitOne,代码照样可以成功运行,因为EndInvoke方法事实上会等待异步操作完成。
	调用" Endlnvoke方法(或者针对其他异步API的EndOperationName方法)是非常重要的,因为该方法会将任何未处理的异常抛回到调用线程中。
	当使用这种异步API时,请确保始终调用了Begin和End方法。
	*/
	class Program
	{
		static void Main(string[] args)
		{
			int threadId = 0;

			RunOnThreadPool poolDelegate = Test;

			//var t = new Thread(() => Test(out threadId));
			//t.Start();
			//t.Join();
			//Console.WriteLine("Thread id: {0}", threadId);

			IAsyncResult r = poolDelegate.BeginInvoke(out threadId, Callback, "a delegate asynchronous call");
			//r.AsyncWaitHandle.WaitOne();
			for (int i = 0; i < 10; i++)
			{
				Console.WriteLine(i);//线程结束前不阻塞主线程
				Thread.Sleep(TimeSpan.FromMilliseconds(100));
			}

			string result = poolDelegate.EndInvoke(out threadId, r);//等待线程结束（阻塞），执行回调
			Console.WriteLine("0000");

			Console.WriteLine("....Thread pool worker thread id: {0}", threadId);
			Console.WriteLine("............"+ result);
			Console.WriteLine("111111");
			Thread.Sleep(TimeSpan.FromSeconds(1));
			Console.WriteLine("222222");


			Thread.Sleep(TimeSpan.FromSeconds(4));




			Console.ReadLine();
		}

		private delegate string RunOnThreadPool(out int threadId);

		private static void Callback(IAsyncResult ar)
		{
			Console.WriteLine("Starting a callback...");
			Console.WriteLine("State passed to a callbak: {0}", ar.AsyncState);
			Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
			Console.WriteLine("Thread pool worker thread id: {0}", Thread.CurrentThread.ManagedThreadId);
		}


		private static string Test(out int threadId)
		{
			Console.WriteLine("Starting...");
			Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
			Thread.Sleep(TimeSpan.FromSeconds(2));
			threadId = Thread.CurrentThread.ManagedThreadId;
			return string.Format("Thread pool worker thread id was: {0}", threadId);
		}
	}
}
