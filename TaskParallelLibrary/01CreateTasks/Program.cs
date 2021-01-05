using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01CreateTasks
{
	class Program
	{
		static void Main(string[] args)
		{
			var t1 = new Task(() => TaskMethod("Task 1"));
			var t2 = new Task(() => TaskMethod("Task 2"));
			t2.Start();
			t1.Start();
			Task.Run(() => TaskMethod("Task 3"));
			Task.Factory.StartNew(() => TaskMethod("Task 4"));

			//Task 5 标记该任务为长时间运行,结果该任务将不会使用线程池,而在单独的线程中运行。
			//然而,根据运行该任务的当前的任务调度程序(task scheduler),运行方式有可能不同。
			Task.Factory.StartNew(() => TaskMethod("Task 5"), TaskCreationOptions.LongRunning);
			Thread.Sleep(TimeSpan.FromSeconds(1));

			Console.ReadLine();
		}

		static void TaskMethod(string name)
		{
			Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
				name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
		}
	}
}
