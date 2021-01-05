using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _08TaskWhenAll
{
	/*
	 * 当程序启动时,创建了两个任务。然后借助于Task.WhenAll方法,创建了第三个任务,该任务将会在所有任务完成后运行。
	 * 该任务的结果提供了一个结果数组,第一个元素是第一个任务的结果,第二个元素是第二个任务的结果,以此类推。
	 * 然后我们创建了另外一系列任务,并使用Task. WhenAny方法等待这些任务中的任何一个完成。
	 * 当有一个完成任务后,从列表中移除该任务并继续等待其他任务完成,直到列表为空。
	 * 获取任务的完成进展情况或在运行任务时使用超时,都可以使用Task.WhenAny方法。
	 * 例如,我们等待一组任务运行,并且使用其中一个任务用来记录是否超时。如果该任务先完成,则只需取消掉其他还未完成的任务。
	 */
	class Program
    {
		static void Main(string[] args)
		{
			var firstTask = new Task<int>(() => TaskMethod("First Task", 3));
			var secondTask = new Task<int>(() => TaskMethod("Second Task", 2));
			var whenAllTask = Task.WhenAll(firstTask, secondTask);

			whenAllTask.ContinueWith(t =>
				Console.WriteLine("The first answer is {0}, the second is {1}", t.Result[0], t.Result[1]),
				TaskContinuationOptions.OnlyOnRanToCompletion
				);

			firstTask.Start();
			secondTask.Start();

			Thread.Sleep(TimeSpan.FromSeconds(4));

			var tasks = new List<Task<int>>();
			for (int i = 1; i < 4; i++)
			{
				int counter = i;
				var task = new Task<int>(() => TaskMethod(string.Format("Task {0}", counter), counter));
				tasks.Add(task);
				task.Start();
			}

			while (tasks.Count > 0)
			{
				var completedTask = Task.WhenAny(tasks).Result;
				tasks.Remove(completedTask);
				Console.WriteLine("A task has been completed with result {0}.", completedTask.Result);
			}

			Thread.Sleep(TimeSpan.FromSeconds(1));
		}

		static int TaskMethod(string name, int seconds)
		{
			Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
				name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
			Thread.Sleep(TimeSpan.FromSeconds(seconds));
			return 42 * seconds;
		}
	}
}
