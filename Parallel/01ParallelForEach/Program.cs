using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01ParallelForEach
{
	/*
	 * 该程序演示了Parallel类的不同功能。与在任务并行库中定义任务的方式相比,调用, Invoke方法可以免去很多麻烦就可实现并行地运行多个任务。
	 * Invoke方法会阻塞其他线程直到所有的任务都被完成,这是一个非常常见的方面使用Invoke方法的场景。
	 * 
	 * 下一个功能是并行循环,使用For和ForEach方法来定义循环。由ForEach方法与For方法非常相似,我们将仔细讲解ForEach方法。
	 * 并行ForEach循环可以通过给每个集合项应用一个action委托的方式,实现并行地处理任何IEnumerable集合。
	 * 我们可以提供几种选项,自定义并行行为,并得到一个结果来说明循环是否成功完成。
	 * 
	 * 可以给ForEach方法提供一个ParallelOptions类的实例来控制并行循环。其允许我们使用CancellationToken取消循环,限制最大并行度(并行运行的最大操作数),
	 * 还可以提供一个自定义的TaskScheduler类来调度任务。Action可以接受一个附加的Parallelloopstate参数,可用于从循环中跳出或者检查当前循环的状态。
	 * 
	 * 使用ParallelLoopState有两种方式停止并行循环。既可以使用Break方法,也可以使用Stop方法。
	 * Stop方法告诉循环停止处理任何工作,并设置并行循环状态属性IsStopped值为true, Break方法停止其之后的迭代,但之前的迭代还要继续工作。
	 * 在那种情况下,循环结果的LowestBreakIteration属性将会包含当Break方法被调用时的最低循环次数。
	 * 
	 */
	class Program
	{
		static void Main(string[] args)
		{
			Parallel.Invoke(
				() => EmulateProcessing("Task1"),
				() => EmulateProcessing("Task2"),
				() => EmulateProcessing("Task3")
			);
			Console.WriteLine("...");
			Console.ReadLine();

			var cts = new CancellationTokenSource();

			var result = Parallel.ForEach(
				Enumerable.Range(1, 30),
				new ParallelOptions
				{
					CancellationToken = cts.Token,
					MaxDegreeOfParallelism = Environment.ProcessorCount,
					TaskScheduler = TaskScheduler.Default
				},
				(i, state) =>
				{
					Console.WriteLine(i);
					if (i == 20)
					{
						state.Break();
						//state.Stop();
						Console.WriteLine("Loop is stopped: {0}", state.IsStopped);
					}
				});

			Console.WriteLine("---");
			Console.WriteLine("IsCompleted: {0}", result.IsCompleted);
			Console.WriteLine("Lowest break iteration: {0}", result.LowestBreakIteration);

			Console.ReadLine();
		}

		static string EmulateProcessing(string taskName)
		{
			Thread.Sleep(TimeSpan.FromMilliseconds(
				new Random(DateTime.Now.Millisecond).Next(250, 350)));
			Console.WriteLine("{0} task was processed on a thread id {1}",
					taskName, Thread.CurrentThread.ManagedThreadId);
			return taskName;
		}
	}
}
