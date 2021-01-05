using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _03ThreadAndPoolParalle
{
	class Program//线程池与并行度
	{
		static void Main(string[] args)
		{
			const int numberOfOperations = 500;
			var sw = new Stopwatch();
			sw.Start();
			UseThreads(numberOfOperations);
			sw.Stop();
			Console.WriteLine("Execution time using threads: {0}", sw.ElapsedMilliseconds);

			sw.Reset();
			sw.Start();
			UseThreadPool(numberOfOperations);
			sw.Stop();
			Console.WriteLine("Execution time using threads: {0}", sw.ElapsedMilliseconds);

			Console.ReadLine();
		}



		//System.Threading.CountdownEvent 是一个同步基元，它在收到一定次数的信号之后，将会解除对其等待线程的锁定。
		//CountdownEvent 专门用于以下情况：您必须使用 ManualResetEvent 或 ManualResetEventSlim，并且必须在用信号通知事件之前手动递减一个变量。
		//例如，在分叉/联接方案中，您可以只创建一个信号计数为 5 的 CountdownEvent，然后在线程池上启动五个工作项，并且让每个工作项在完成时调用 Signal。 
		//每次调用 Signal 时，信号计数都会递减 1。 在主线程上，对 Wait 的调用将会阻塞，直至信号计数为零。

		//简单的说就是先设置多少个数量，然后Signal通知一个已经完成，Wait等待所有数量全部完成，则继续往下运行
		static void UseThreads(int numberOfOperations)
		{
			using (var countdown = new CountdownEvent(numberOfOperations))
			{
				Console.WriteLine("Scheduling work by creating threads");
				for (int i = 0; i < numberOfOperations; i++)
				{
					var thread = new Thread(() => {
						Console.Write("{0},", Thread.CurrentThread.ManagedThreadId);
						Thread.Sleep(TimeSpan.FromSeconds(0.1));
						countdown.Signal();
					});
					thread.Start();
				}
				countdown.Wait();//主线程上，对 Wait 的调用将会阻塞
				Console.WriteLine("------------------------------------------------");
			}
		}

		static void UseThreadPool(int numberOfOperations)
		{
			using (var countdown = new CountdownEvent(numberOfOperations))
			{
				Console.WriteLine("Starting work on a threadpool");
				for (int i = 0; i < numberOfOperations; i++)
				{
					ThreadPool.QueueUserWorkItem(_ => {
						Console.Write("{0},", Thread.CurrentThread.ManagedThreadId);
						Thread.Sleep(TimeSpan.FromSeconds(0.1));
						countdown.Signal();
					});
				}
				countdown.Wait();//主线程上，对 Wait 的调用将会阻塞
				Console.WriteLine("------------------------------------------------");
			}
		}
	}
}
