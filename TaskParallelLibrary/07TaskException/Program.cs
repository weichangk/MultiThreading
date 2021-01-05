using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _07TaskException
{
	/*
	 * 当程序启动时,创建了一个任务并尝试同步获取任务结果。Result属性的Get部分会使当前线程等待直到该任务完成,并将异常传播给当前线程。
	 * 在这种情况下,通过catch代码块可以很容易地捕获异常,但是该异常是一个被封装的异常,叫做AggregateException。
	 * 在本例中,它里面包含一个异常,因为只有一个任务抛出了异常。可以访问InnerException属性来得到底层异常。
	 * 第二个例子与第一个非常相似,不同之处是使用GetAwaiter和GetResult方法来访问任务结果。
	 * 这种情况下,无需封装异常,因为TPL基础设施会提取该异常。如果只有一个底层任务,那么一次只能获取一个原始异常,这种设计非常合适。
	 * 最后一个例子展示了两个任务抛出异常的情形。现在使用后续操作来处理异常。只有之前的任务完成前有异常时,该后续操作才会被执行。
	 * 通过给后续操作传递TaskContinuationoptions.OnlyOnFaulted选项可以实现该行为。结果打印出了AggregateException,其内部封装了两个任务抛出的异常
	 */
	class Program
    {
		static void Main(string[] args)
		{
			Task<int> task;
			try
			{
				task = Task.Run(() => TaskMethod("Task 1", 2));
				int result = task.Result;
				Console.WriteLine("Result: {0}", result);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception caught: {0}", ex);
			}
			Console.WriteLine("----------------------------------------------");
			Console.WriteLine();

			try
			{
				task = Task.Run(() => TaskMethod("Task 2", 2));
				int result = task.GetAwaiter().GetResult();
				Console.WriteLine("Result: {0}", result);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception caught: {0}", ex);
			}
			Console.WriteLine("----------------------------------------------");
			Console.WriteLine();

			var t1 = new Task<int>(() => TaskMethod("Task 3", 3));
			var t2 = new Task<int>(() => TaskMethod("Task 4", 2));
			var complexTask = Task.WhenAll(t1, t2);
			var exceptionHandler = complexTask.ContinueWith(t => {
						var ae = t.Exception.Flatten();
						var exceptions = ae.InnerExceptions;
						Console.WriteLine("Exceptions caught: {0}", exceptions.Count);
						foreach (var e in exceptions)
						{
							Console.WriteLine("Exception details: {0}", e);
							Console.WriteLine();
						}
					},
					//Console.WriteLine("Exception caught: {0}", t.Exception),
					TaskContinuationOptions.OnlyOnFaulted
				);
			t1.Start();
			t2.Start();

			Thread.Sleep(TimeSpan.FromSeconds(5));

			Console.ReadLine();
		}

		static int TaskMethod(string name, int seconds)
		{
			Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
				name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
			Thread.Sleep(TimeSpan.FromSeconds(seconds));
			Console.WriteLine(name);
			throw new Exception("Boom!");
			return 42 * seconds;
		}
	}
}
