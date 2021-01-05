﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02AsyncAwaitLambda
{
	class Program
	{
		static void Main(string[] args)
		{
			Task t = AsynchronousProcessing();
			//t.Wait();
			Console.WriteLine("...");
			Console.ReadLine();
		}

		async static Task AsynchronousProcessing()
		{
			Func<string, Task<string>> asyncLambda = async name => {
				await Task.Delay(TimeSpan.FromSeconds(2));
				return string.Format("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
					name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
			};

			string result = await asyncLambda("async lambda");

			Console.WriteLine(result);
		}
	}
}
