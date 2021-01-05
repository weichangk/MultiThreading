using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _03ConcurrentStack
{
	/*
	 * 现在可以看到任务处理的顺序被改变了。堆栈是一个LIFO集合,工作者先处理最近的任务。
	 * 在并发队列中,任务被处理的顺序与被添加的顺序几乎一致。这意味着根据工作者的数量,我们必将在一定时间窗内处理先被创建的任务。
	 * 而在堆栈中,早先创建的任务具有较低的优先级,而且直到生产者停止向堆栈中放入更多任务后,该任务才有可能被处理。
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
			var taskStack = new ConcurrentStack<CustomTask>();
			var cts = new CancellationTokenSource();

			var taskSource = Task.Run(() => TaskProducer(taskStack));

			Task[] processors = new Task[4];
			for (int i = 1; i <= 4; i++)
			{
				string processorId = i.ToString();
				processors[i - 1] = Task.Run(
					() => TaskProcessor(taskStack, "Processor " + processorId, cts.Token));
			}

			await taskSource;
			cts.CancelAfter(TimeSpan.FromSeconds(2));

			await Task.WhenAll(processors);
		}

		static async Task TaskProducer(ConcurrentStack<CustomTask> stack)
		{
			for (int i = 1; i <= 20; i++)
			{
				await Task.Delay(50);
				var workItem = new CustomTask { Id = i };
				stack.Push(workItem);
				Console.WriteLine("Task {0} has been posted", workItem.Id);
			}
		}

		static async Task TaskProcessor(
			ConcurrentStack<CustomTask> stack, string name, CancellationToken token)
		{
			await GetRandomDelay();
			do
			{
				CustomTask workItem;
				bool popSuccesful = stack.TryPop(out workItem);
				if (popSuccesful)
				{
					Console.WriteLine("Task {0} has been processed by {1}", workItem.Id, name);
				}

				await GetRandomDelay();
			}
			while (!token.IsCancellationRequested);
		}

		static Task GetRandomDelay()
		{
			int delay = new Random(DateTime.Now.Millisecond).Next(1, 500);
			return Task.Delay(delay);
		}

		class CustomTask
		{
			public int Id { get; set; }
		}
	}
}
