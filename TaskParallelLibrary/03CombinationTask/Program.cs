using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _03CombinationTask
{
	class Program
	{
		//设置相互依赖的任务,创建一个任务,使其在父任务完成后才会被运行。另外为非常短暂的任务节省线程开销的可能性。
		static void Main(string[] args)
		{
			var firstTask = new Task<int>(() => TaskMethod("First Task", 3));
			var secondTask = new Task<int>(() => TaskMethod("Second Task", 2));

			firstTask.ContinueWith(
				t => Console.WriteLine("The first answer is {0}. Thread id {1}, is thread pool thread: {2}",
					t.Result, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread),
				TaskContinuationOptions.OnlyOnRanToCompletion);//后续操作，First Task任务完成后执行

			firstTask.Start();
			secondTask.Start();

			Thread.Sleep(TimeSpan.FromSeconds(4)); //两个任务并等待4秒,这个时间足够两个任务完成。

			//然后给第二个任务运行另一个后续操作,并通过指定TaskContinuationOptions.ExecuteSynchronously选项来尝试同步执行该后续操作。
			//如果后续操作耗时非常短暂!!!!!!!!!!!!1使用该方式是非常有用的,因为放置在主线程中运行比放置在线程池中运行要快。
			//可以实现这一点是因为第二个任务恰好在那刻完成。如果注释掉4秒的Thread.Sleep方法,将会看到该代码被放置到线程池中,这是因为还未从之前的任务中得到结果。
			Task continuation = secondTask.ContinueWith(
				t => Console.WriteLine("The second answer is {0}. Thread id {1}, is thread pool thread: {2}",
					t.Result, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread),
				TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);


			continuation.GetAwaiter().OnCompleted(
				() => Console.WriteLine("Continuation Task Completed! Thread id {0}, is thread pool thread: {1}",
					Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread));

			Thread.Sleep(TimeSpan.FromSeconds(2));
			Console.WriteLine();

			firstTask = new Task<int>(() =>
			{
				var innerTask = Task.Factory.StartNew(() => TaskMethod("Second Task", 5), TaskCreationOptions.AttachedToParent);//TaskCreationOptions.AttachedToParent选项来运行子任务。子任务必须在父任务运行时创建,并正确的附加给父任务!
				innerTask.ContinueWith(t => TaskMethod("Third Task", 2), TaskContinuationOptions.AttachedToParent);//子线程的后续操作对于夫线程来说也是子线程
				return TaskMethod("First Task", 2);
			});

			firstTask.Start();

			while (!firstTask.IsCompleted)//只有所有子任务结束工作,父任务才会完成。
			{
				Console.WriteLine(firstTask.Status);
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
			}
			Console.WriteLine(firstTask.Status);

			Thread.Sleep(TimeSpan.FromSeconds(10));

			Console.ReadLine();
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
