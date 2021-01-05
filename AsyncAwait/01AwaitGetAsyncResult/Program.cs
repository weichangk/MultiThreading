using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01AwaitGetAsyncResult
{
	class Program
	{
		static void Main(string[] args)
		{
			Task t;
			//t = AsynchronyWithTPL();//利用回调执行异步结果
			//t.Wait();

			t = AsynchronyWithoutAwait();//没有Await与主线程同步执行

			t = AsynchronyWithAwait();//async与Await关键字实现异步执行，Await等待异步结果
			//t.Wait();//等待Task完成，阻塞
			Console.WriteLine("...");

			Console.ReadLine();
		}

		static Task AsynchronyWithTPL()
		{
			Task<string> t = GetInfoAsync("Task 1");
			Task t2 = t.ContinueWith(task => Console.WriteLine(t.Result),
				TaskContinuationOptions.NotOnFaulted);
			Task t3 = t.ContinueWith(task => Console.WriteLine(t.Exception.InnerException),
				TaskContinuationOptions.OnlyOnFaulted);

			return Task.WhenAny(t2, t3);
		}

		async static Task AsynchronyWithAwait()
		{
			try
			{
				string result = await GetInfoAsync("Task 2");
				Console.WriteLine(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		async static Task<string> GetInfoAsync(string name)
		{
			await Task.Delay(TimeSpan.FromSeconds(2));
			//throw new Exception("Boom!");
			return string.Format("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
				name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
		}

		async static Task AsynchronyWithoutAwait()
		{
			try
			{
				for (int i = 0; i < 5; i++)
				{
					Thread.Sleep(1000);
					Console.WriteLine(i);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		static async Task GetValueAsync()
		{
			await Task.Run(() =>
			{
				Thread.Sleep(1000);
				for (int i = 0; i < 5; ++i)
				{
					Console.Out.WriteLine(String.Format("From task : {0}", i));
				}
			});
			Console.Out.WriteLine("Task End");
		}
	}
}
