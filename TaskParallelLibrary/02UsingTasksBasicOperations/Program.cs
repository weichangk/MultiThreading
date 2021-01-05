using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02UsingTasksBasicOperations
{
	class Program
	{
		//了解在线程池中和主线程中运行任务的不同之处。
		static void Main(string[] args)
		{
			TaskMethod("Main Thread Task");//与主线程同步执行

			Task<int> task = CreateTask("Task 1");
			task.Start();
			int result = task.Result;//使用Start方法启动该任务并等待结果。该任务会被放置在线程池中,并且主线程会等待,直到任务返回前一直处于阻塞状态。
			Console.WriteLine("Result is: {0}", result);

			task = CreateTask("Task 2");
			task.RunSynchronously();//该任务会运行在主线程中,该任务的输出与直接同步调用TaskMethod的输出完全一样。这是个非常好的优化,可以避免使用线程池来执行非常短暂的操作。
			result = task.Result;
			Console.WriteLine("Result is: {0}", result);

			task = CreateTask("Task 3");
			Console.WriteLine(task.Status);
			task.Start();//使用Start方法启动该任务,没有阻塞

			while (!task.IsCompleted)
			{
				Console.WriteLine(task.Status);
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
			}

			Console.WriteLine(task.Status);
			result = task.Result;
			Console.WriteLine("Result is: {0}", result);

			Console.ReadLine();
		}

		static Task<int> CreateTask(string name)
		{
			return new Task<int>(() => TaskMethod(name));
		}

		static int TaskMethod(string name)
		{
			Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
				name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
			Thread.Sleep(TimeSpan.FromSeconds(2));
			return 42;
		}
	}
}
