using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _04APMConversionTask
{
	/*
     * APM即异步编程模型的简写（Asynchronous Programming Model），大家在写代码的时候或者查看.NET 的类库的时候肯定会经常看到和使用以BeginXXX和EndXXX类似的方法，
     * 其实你在使用这些方法的时候，你就再使用异步编程模型来编写程序。异步编写模型是一种模式，该模式允许用更少的线程去做更多的操作，
     * .NET Framework很多类也实现了该模式，同时我们也可以自定义类来实现该模式，（也就是在自定义的类中实现返回类型为IAsyncResult接口的BeginXXX方法和EndXXX方法），
     * 另外委托类型也定义了BeginInvoke和EndInvoke方法，并且我们使用WSDL.exe和SvcUtil.exe工具来生成Web服务的代理类型时，也会生成使用了APM的BeginXxx和EndXxx方法。
	 * 
	 * 将APM转换为TPL的关键点是Task<TD>.Factory.FromAsync方法, T是异步操作结果的类,型。该方法有数个重载。
	 * 在第一个例子中传入了IAsyncResult和FuncsIAsyncResult, string>,这是一个将IAsyncResult的实现作为参数并返回一个字符串的方法。由于第一个委托类型提供的EndMethod与该签名是兼容的,所以将该委托的异步调用转换为任务没有任何问题。
	 * 
	 * 第二个例子做的事与第一个非常相似,但是使用了不同的FromAsync方法重载,该重载并不允许指定一个将会在异步委托调用完成后被调用的回调函数。
	 * 但我们可以使用后续操作替代它。但如果回调函数很重要,可以使用第一个例子所示的方法。
	 * 
	 * 最后一个例子展示了一个小技巧。这次IncompatibleAsynchronousTask委托的EndMethod使用了out参数,与FromAsync方法重载并不兼容。
	 * 然而,可以很容易地将EndMethod调用封装到一个lambda表达式中,从而适合任务工厂方法。
     */
	class Program
	{
		private static void Main(string[] args)
		{
			int threadId;
			AsynchronousTask d = Test;
			IncompatibleAsynchronousTask e = Test;

			Console.WriteLine("Option 1");
			Task<string> task = Task<string>.Factory.FromAsync(
				d.BeginInvoke("AsyncTaskThread", Callback, "a delegate asynchronous call"), d.EndInvoke);

			task.ContinueWith(t => Console.WriteLine("Callback is finished, now running a continuation! Result: {0}",
				t.Result));

			while (!task.IsCompleted)
			{
				Console.WriteLine(task.Status);
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
			}
			Console.WriteLine(task.Status);
			Thread.Sleep(TimeSpan.FromSeconds(1));

			Console.WriteLine("----------------------------------------------");
			Console.WriteLine();
			Console.WriteLine("Option 2");

			task = Task<string>.Factory.FromAsync(
				d.BeginInvoke, d.EndInvoke, "AsyncTaskThread", "a delegate asynchronous call");
			task.ContinueWith(t => Console.WriteLine("Task is completed, now running a continuation! Result: {0}",
				t.Result));
			while (!task.IsCompleted)
			{
				Console.WriteLine(task.Status);
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
			}
			Console.WriteLine(task.Status);
			Thread.Sleep(TimeSpan.FromSeconds(1));

			Console.WriteLine("----------------------------------------------");
			Console.WriteLine();
			Console.WriteLine("Option 3");

			IAsyncResult ar = e.BeginInvoke(out threadId, Callback, "a delegate asynchronous call");
			task = Task<string>.Factory.FromAsync(ar, _ => e.EndInvoke(out threadId, ar));
			task.ContinueWith(t =>
				Console.WriteLine("Task is completed, now running a continuation! Result: {0}, ThreadId: {1}",
					t.Result, threadId));

			while (!task.IsCompleted)
			{
				Console.WriteLine(task.Status);
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
			}
			Console.WriteLine(task.Status);

			Thread.Sleep(TimeSpan.FromSeconds(1));

			Console.ReadLine();
		}

		private delegate string AsynchronousTask(string threadName);
		private delegate string IncompatibleAsynchronousTask(out int threadId);

		private static void Callback(IAsyncResult ar)
		{
			Console.WriteLine("Starting a callback...");
			Console.WriteLine("State passed to a callbak: {0}", ar.AsyncState);
			Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
			Console.WriteLine("Thread pool worker thread id: {0}", Thread.CurrentThread.ManagedThreadId);
		}

		private static string Test(string threadName)
		{
			Console.WriteLine("Starting...");
			Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
			Thread.Sleep(TimeSpan.FromSeconds(2));
			Thread.CurrentThread.Name = threadName;
			return string.Format("Thread name: {0}", Thread.CurrentThread.Name);
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
