using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _07AsyncVoid
{
	/*
	 * 本节描述了为什么使用async void方法非常危险。我们将学习在哪种情况下可使用该方法,以及如何尽可能地替代该方法。
	 * 
	 * 当程序启动时,我们通过调用AsyncTask和AsyncVoid这两个方法启动了两个异步操作。
	 * 第一个方法返回一个Task对象,而另一个由于被声明为async void所以没有返回值。
	 * 由于它们都是异步的所以都会立即返回。但是第一个方法通过返回的任务状态或对其调用Wait方法从而很容易实现监控。
	 * 等待第二个方法完成的唯一方式是确切地等待多长时间,因为我们没有声明任何对象可以监控该异步操作的状态。
	 * 当然可以使用某种共享的状态变量,将其设置到async void方法中,并从调用方法中检查其值,但返回一个Task对象的方式更好些。
	 * 
	 * 最危险的部分是异常处理。使用async void方法,异常处理方法将被放置到当前的同步上下文中,在本例中即线程池中。
	 * 线程池中未被处理的异常会终结整个进程。使用AppDomain.UnhandledException事件可以拦截未被处理的异常,但不能从拦截的地方恢复进程。
	 * 为了重现该场景,可以取消Main方法中对try/catch代码块的注释,然后运行程序。
	 * 
	 * 关于使用async void lambda表达式的另一个事实是:它们与Action类型是兼容的,而Action类型在标准 NET Framework类库中的使用非常广泛。
	 * 在lambda表达式中很容易忘记对异常的处理,这将再次导致程序崩溃。可以取消在Main方法中第二个被注释的代码块的注释来重现该场景。
	 * 强烈建议只在U1事件处理器中使用async void方法。在其他所有的情况下,请使用返回Task的方法。
	 */
	class Program
	{
		static void Main(string[] args)
		{
			Task t = AsyncTask();
			t.Wait();

			AsyncVoid();
			//Thread.Sleep(TimeSpan.FromSeconds(3));
			Console.WriteLine("...");
			Console.ReadLine();

			t = AsyncTaskWithErrors();
			while (!t.IsFaulted)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
			Console.WriteLine(t.Exception);

			//try
			//{
			//	AsyncVoidWithErrors();
			//	Thread.Sleep(TimeSpan.FromSeconds(3));
			//}
			//catch (Exception ex)
			//{
			//	Console.WriteLine(ex);
			//}

			int[] numbers = new[] { 1, 2, 3, 4, 5 };
			Array.ForEach(numbers, async number => {
				await Task.Delay(TimeSpan.FromSeconds(1));
				if (number == 3) throw new Exception("Boom!");
				Console.WriteLine(number);
			});

			Console.ReadLine();
		}

		async static Task AsyncTaskWithErrors()
		{
			string result = await GetInfoAsync("AsyncTaskException", 2);
			Console.WriteLine(result);
		}

		async static void AsyncVoidWithErrors()
		{
			string result = await GetInfoAsync("AsyncVoidException", 2);
			Console.WriteLine(result);
		}

		async static Task AsyncTask()
		{
			string result = await GetInfoAsync("AsyncTask", 2);
			Console.WriteLine(result);
		}

		private static async void AsyncVoid()
		{
			string result = await GetInfoAsync("AsyncVoid", 2);
			Console.WriteLine(result);
		}

		async static Task<string> GetInfoAsync(string name, int seconds)
		{
			await Task.Delay(TimeSpan.FromSeconds(seconds));
			if (name.Contains("Exception"))
				throw new Exception(string.Format("Boom from {0}!", name));
			return string.Format("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
				name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
		}
	}
}
