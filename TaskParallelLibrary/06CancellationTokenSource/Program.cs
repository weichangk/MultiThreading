using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _06CancellationTokenSource
{
	/*
	 * longTask的创建代码。我们将给底层任务传递一次取消标志,然后给任务构造函数再传递一次。为什么需要提供取消标志两次呢?
	 * 答案是如果在任务实际启动前取消它,该任务的TPL基础设施有责任处理该取消操作,因为这些代码根本不会执行。
	 * 通过得到的第一个任务的状态可以知道它被取消了。如果尝试对该任务调用Start方法,将会得到InvalidOperationException异常。
	 * 然后需要自己写代码来处理取消过程。这意味着我们对取消过程全权负责,并且在取消任务后,任务的状态仍然是RanToCompletion,
	 * 因为从TPL的视角来看,该任务正常完成了它的工作。辨别这两种情况是非常重要的,并且需要理解每种情况下职责的不同。
	 */
	class Program
    {
		private static void Main(string[] args)
		{
			var cts = new CancellationTokenSource();
			var longTask = new Task<int>(() => TaskMethod("Task 1", 10, cts.Token), cts.Token);
			Console.WriteLine(longTask.Status);
			cts.Cancel();
			Console.WriteLine(longTask.Status);
			Console.WriteLine("First task has been cancelled before execution");
			cts = new CancellationTokenSource();
			longTask = new Task<int>(() => TaskMethod("Task 2", 10, cts.Token), cts.Token);
			longTask.Start();
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
				Console.WriteLine(longTask.Status);
			}
			cts.Cancel();
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
				Console.WriteLine(longTask.Status);
			}

			Console.WriteLine("A task has been completed with result {0}.", longTask.Result);




			var cts1 = new CancellationTokenSource();
			var longTask1 = new Task<int>(() => TaskMethod("Task 3", 10, cts1.Token));
			cts1.Cancel();
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
				Console.WriteLine(longTask1.Status);
			}
			longTask1.Start();
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
				Console.WriteLine(longTask1.Status);
			}
			Console.WriteLine("Task 3 has been completed with result {0}.", longTask1.Result);

			Console.ReadLine();
		}

		private static int TaskMethod(string name, int seconds, CancellationToken token)
		{
			Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
				name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
			for (int i = 0; i < seconds; i++)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
				Console.WriteLine("...");
				if (token.IsCancellationRequested) return -1;
			}
			return 42 * seconds;
		}
	}
}
