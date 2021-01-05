using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _04CancellationTokenSource
{
	//在线程池中取消异步操作
	//例子描述了如何在线程池中实现一个取消机制。
	//使用CancellationTokenSource和CancellationToken类，他们是在.NET4被引入的，目前是实现异步操作的取消操作的事实标准。
	//例子使用了三种方式来实现取消过程。
	//第一个是轮询查询CancellationToken.IsCancellationRequested属性，如果为true则说明异步操作需要被取消。
	//第二种是抛出一个CancellationToken.ThrowIfCancellationRequested异常。这允许在操作之外控制取消过程，即需要取消操作时，通过操作之外的代码来处理。
	//第三种是注册一个回调函数。当操作被取消时，在线程池将调用该回调函数。这允许链式传递一个取消逻辑到另一个异步操作中。
	class Program
	{
		static void Main(string[] args)
		{
			//using (var cts = new CancellationTokenSource())
			//{
			//	CancellationToken token = cts.Token;
			//	ThreadPool.QueueUserWorkItem(_ => AsyncOperation1(token));
			//	Thread.Sleep(TimeSpan.FromSeconds(3));
			//	cts.Cancel();
			//}

			//using (var cts = new CancellationTokenSource())
			//{
			//	CancellationToken token = cts.Token;
			//	ThreadPool.QueueUserWorkItem(_ => AsyncOperation2(token));
			//	Thread.Sleep(TimeSpan.FromSeconds(2));
			//	cts.Cancel();
			//}

			//using (var cts = new CancellationTokenSource())
			//{
			//	CancellationToken token = cts.Token;
			//	ThreadPool.QueueUserWorkItem(_ => AsyncOperation3(token));
			//	Thread.Sleep(TimeSpan.FromSeconds(2));
			//	cts.Cancel();
			//}

			using (var cts = new CancellationTokenSource())
			{
				CancellationToken token = cts.Token;
				ThreadPool.QueueUserWorkItem(_ => AsyncOperation4(token));
				Thread.Sleep(TimeSpan.FromSeconds(2));
				cts.Cancel();
			}

			//Thread.Sleep(TimeSpan.FromSeconds(3));

			Console.ReadLine();
		}

		static void AsyncOperation1(CancellationToken token)
		{
			Console.WriteLine("Starting the first task");
			for (int i = 0; i < 5; i++)
			{
				if (token.IsCancellationRequested)
				{
					Console.WriteLine("The first task has been canceled.");
					return;
				}
				Console.WriteLine(i);
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
			Console.WriteLine("The first task has completed succesfully");
		}

		static void AsyncOperation2(CancellationToken token)
		{
			try
			{
				Console.WriteLine("Starting the second task");

				for (int i = 0; i < 5; i++)
				{
					token.ThrowIfCancellationRequested();
					Thread.Sleep(TimeSpan.FromSeconds(1));
					Console.WriteLine(i);
				}
				Console.WriteLine("The second task has completed succesfully");
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine("The second task has been canceled.");
			}
		}

		private static void AsyncOperation3(CancellationToken token)
		{
			bool cancellationFlag = false;
			token.Register(() => cancellationFlag = true);
			Console.WriteLine("Starting the third task");
			for (int i = 0; i < 5; i++)
			{
				if (cancellationFlag)
				{
					Console.WriteLine("The third task has been canceled.");
					return;
				}
				Thread.Sleep(TimeSpan.FromSeconds(1));
				Console.WriteLine(i);
			}
			Console.WriteLine("The third task has completed succesfully");
		}

		private static void AsyncOperation4(CancellationToken token)
		{
			//主线程执行cts.Cancel();调用了回调函数，但是子线程还是没被终止还是会继续执行。
			//如果主线程执行cts.Cancel()后需要退出子线程应该使用AsyncOperation3方法加入状态标志退出子线程。
			token.Register(() => Console.WriteLine("The 4 task has been canceled.")); 
			Console.WriteLine("Starting the third task");
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
				Console.WriteLine(i);
			}
			Console.WriteLine("The third task has completed succesfully");
		}
	}
}

