using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02ConcurrentQueue
{
	/*
	 * 当程序运行时,我们使用ConcurrentQueue集合实例创建了一个任务队列。然后创建了一个取消标志,它是用来在我们将任务放入队列后停止工作的。
	 * 接下来启动了一个单独的工作线程来将任务放入任务队列中。该部分为异步处理产生了工作量。
	 * 
	 * 现在定义该程序中消费任务的部分。我们创建了四个工作者,它们会随机等待一段时间,然后从任务队列中获取一个任务,处理该任务,一直重复整个过程直到我们发出取消标志信号。
	 * 最后,我们启动产生任务的线程,等待该线程完成。然后使用取消标志给消费者发信号我们完成了工作。最后一步将等待所有的消费者完成。
	 * 
	 * 我们看到队列中的任务按从前到后的顺序被处理,但一个后面的任务是有可能会比前面的任务先处理的,因为我们有四个工作者独立地运行,而且任务处理时间并不是恒定的。
	 * 我们看到访问该队列是线程安全的,没有一个元素会被提取两次。
	 */
	class Program
	{
		static void Main(string[] args)
		{
			Task t = RunProgram();
			t.Wait();

			Console.ReadLine();
		}

		static async Task RunProgram()
		{
			var taskQueue = new ConcurrentQueue<CustomTask>();
			var cts = new CancellationTokenSource();

			var taskSource = Task.Run(() => TaskProducer(taskQueue));

			Task[] processors = new Task[4];
			for (int i = 1; i <= 4; i++)
			{
				string processorId = i.ToString();
				processors[i - 1] = Task.Run(
					() => TaskProcessor(taskQueue, "Processor " + processorId, cts.Token));
			}

			await taskSource;
			cts.CancelAfter(TimeSpan.FromSeconds(2));

			await Task.WhenAll(processors);
		}

		static async Task TaskProducer(ConcurrentQueue<CustomTask> queue)
		{
			for (int i = 1; i <= 20; i++)
			{
				await Task.Delay(50);
				var workItem = new CustomTask { Id = i };
				queue.Enqueue(workItem);
				Console.WriteLine("Task {0} has been posted", workItem.Id);
			}
		}

		static async Task TaskProcessor(
			ConcurrentQueue<CustomTask> queue, string name, CancellationToken token)
		{
			CustomTask workItem;
			bool dequeueSuccesful = false;

			await GetRandomDelay();
			do
			{
				dequeueSuccesful = queue.TryDequeue(out workItem);
				if (dequeueSuccesful)
				{
					Console.WriteLine("Task {0} has been processed by {1}", workItem.Id, name);
				}

				await GetRandomDelay();
				Console.WriteLine(name + "...");
			}
			while (!token.IsCancellationRequested);
		}

		static Task GetRandomDelay()
		{
			int delay = new Random(DateTime.Now.Millisecond).Next(1, 500);//如果大于1秒(2- 20*50)有可能消费之前消费任务就取消了
			return Task.Delay(delay);
		}

		class CustomTask
		{
			public int Id { get; set; }
		}
	}
}
