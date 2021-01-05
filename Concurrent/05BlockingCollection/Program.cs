using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _05BlockingCollection
{
	/*
	 * 使用BlockingCollection来简化实现异步处理的工作负载。
	 * 
	 * 先说第一个场景,这里我们使用了BlockingCollection类,它带来了很多优势。首先,我们能够改变任务存储在阻塞集合中的方式。
	 * 默认情况下它使用的是ConcurrentQueue容器,但是我们能够使用任何实现了IProducerConsumerCollection泛型接口的集合。
	 * 为了演示该点,我们运行了该程序两次,第二次时使用ConcurrentStack作为底层集合。
	 * 
	 * 工作者通过对阻塞集合迭代调用GetConsumingEnumerable方法来获取工作项。
	 * 如果在该集合中没有任何元素,迭代器会阻塞工作线程直到有元素被放置到集合中。
	 * 当生产者调用集合的CompleteAdding时该迭代周期会结束。这标志着工作完成了。
	 * 
	 * 这里很容易犯一个错误,即对BlockingCollection进行迭代,因为它自身实现了IEnumerable,接口。
	 * 不要忘记使用GetConsumingEnumerable,否则你迭代的只是集合的“快照”,这并不是期望的程序行为。
	 * 
	 * 工作量生产者将任务插入到BlockingCollection然后调用CompleteAdding方法,这会使所有工作者完成工作。
	 * 现在在程序输出中我们看到两个结果序列,演示了并发队列和堆栈集合的不同之处。
	 */
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Using a Queue inside of BlockingCollection");
			Console.WriteLine();
			Task t = RunProgram();
			t.Wait();

			Console.WriteLine();
			Console.WriteLine("Using a Stack inside of BlockingCollection");
			Console.WriteLine();
			t = RunProgram(new ConcurrentStack<CustomTask>());
			t.Wait();
		}

		static async Task RunProgram(IProducerConsumerCollection<CustomTask> collection = null)
		{
			var taskCollection = new BlockingCollection<CustomTask>();
			if (collection != null)
				taskCollection = new BlockingCollection<CustomTask>(collection);

			var taskSource = Task.Run(() => TaskProducer(taskCollection));

			Task[] processors = new Task[4];
			for (int i = 1; i <= 4; i++)
			{
				string processorId = "Processor " + i;
				processors[i - 1] = Task.Run(
					() => TaskProcessor(taskCollection, processorId));
			}

			await taskSource;

			await Task.WhenAll(processors);
		}

		static async Task TaskProducer(BlockingCollection<CustomTask> collection)
		{
			for (int i = 1; i <= 20; i++)
			{
				await Task.Delay(20);
				var workItem = new CustomTask { Id = i };
				collection.Add(workItem);
				Console.WriteLine("Task {0} has been posted", workItem.Id);
			}
			collection.CompleteAdding();
		}

		static async Task TaskProcessor(
			BlockingCollection<CustomTask> collection, string name)
		{
			await GetRandomDelay();
			foreach (CustomTask item in collection.GetConsumingEnumerable())
			{
				Console.WriteLine("Task {0} has been processed by {1}", item.Id, name);
				await GetRandomDelay();
			}
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
