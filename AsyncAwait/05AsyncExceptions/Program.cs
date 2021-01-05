using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _05AsyncExceptions
{
	/*
	 * 第一种情况是最简单的,并且与常见的同步代码几乎完全一样。我们只使用try/catch声明即可获取异常细节。
	 * 一个很常见的错误是对一个以上的异步操作使用await时还使用以上方式。如果仍像第一种情况一样使用catch代码块,则只能从底层的AggregateException对象中得到第一个异常。
	 * 为了收集所有异常信息,可以使用await任务的Exception属性。
	 * 在第三种情况中,使用AggregateException的Flatten方法将层级异常放入一个列表,并且从中提取出所有的底层异常。
	 */
	class Program
	{
		static void Main(string[] args)
		{
			Task t = AsynchronousProcessing();
			t.Wait();

			Console.ReadLine();
		}

		async static Task AsynchronousProcessing()
		{
			Console.WriteLine("1. Single exception");

			try
			{
				string result = await GetInfoAsync("Task 1", 2);
				Console.WriteLine(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception details: {0}", ex);
			}

			Console.WriteLine();
			Console.WriteLine("2. Multiple exceptions");

			Task<string> t1 = GetInfoAsync("Task 1", 3);
			Task<string> t2 = GetInfoAsync("Task 2", 2);
			try
			{
				string[] results = await Task.WhenAll(t1, t2);
				Console.WriteLine(results.Length);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception details: {0}", ex);
			}

			Console.WriteLine();
			Console.WriteLine("2. Multiple exceptions with AggregateException");

			t1 = GetInfoAsync("Task 1", 3);
			t2 = GetInfoAsync("Task 2", 2);
			Task<string[]> t3 = Task.WhenAll(t1, t2);
			try
			{
				string[] results = await t3;
				Console.WriteLine(results.Length);
			}
			catch
			{
				var ae = t3.Exception.Flatten();
				var exceptions = ae.InnerExceptions;
				Console.WriteLine("Exceptions caught: {0}", exceptions.Count);
				foreach (var e in exceptions)
				{
					Console.WriteLine("Exception details: {0}", e);
					Console.WriteLine();
				}
			}
		}

		async static Task<string> GetInfoAsync(string name, int seconds)
		{
			await Task.Delay(TimeSpan.FromSeconds(seconds));
			throw new Exception(string.Format("Boom from {0}!", name));
		}
	}
}
