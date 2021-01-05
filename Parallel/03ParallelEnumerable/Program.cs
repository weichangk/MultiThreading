using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _03ParallelEnumerable
{
	/*
	 * 该程序演示了多个可供程序员使用的非常有用的PLINQ选项。我们先创建了一个PLINQ查询,然后又创建了一个调整了PLINQ选项的查询。
	 * 先从取消选项开始。接受一个取消标志对象的WithCancellation方法可用于取消查询。
	 * 本例中我们三秒后发出取消标志信号,这导致查询中抛出OperationCanceledException异常,并且取消了剩余的工作。
	 * 
	 * 然后可以为查询指定并行度。这是被用于执行查询时实际并行分割数。在第一节中我们使用了Parallel.ForEach循环,其拥有最大并行度选项。
	 * 该选项与本节中的不一样,这是因为它指定了一个最大的分割值,但如果基础设施决定最好使用较少的并行度以节省资源和提高性能,那么并行度会小于最大值。
	 * 
	 * 另一个有意思的选项是使用WithExecutionMode方法来重载查询执行的模式。PLINQ基础设施如果认为并行化某查询只会增加工作量并且运行更慢,那么将会以顺序模式执行该查询。
	 * 但我们可以强制该查询以并行的方式运行。可以使用WithMergeOptions方法调整对查询结果的处理。
	 * 默认模式是PLINQ基础设施在查询返回结果之前会缓存一定数量的结果。如果查询花费了大量的时间,更合理的方式是关闭结果缓存从而尽可能快地得到结果。
	 * 
	 * 最后一个选项是AsOrdered方法。当使用并行执行时,集合中的项有可能不是被顺序处理的。集合中稍后的项可能会比稍前的项先处理。
	 * 为了防止该情况,我们可以显式的对并行查询调用AsOrdered方法,来告诉PLINQ基础设施我们打算按项在集合中的顺序来进行处理。
	 */
	class Program
	{
		static void Main(string[] args)
		{
			var parallelQuery = from t in GetTypes().AsParallel()
								select EmulateProcessing(t);

			var cts = new CancellationTokenSource();
			cts.CancelAfter(TimeSpan.FromSeconds(3));

			try
			{
				parallelQuery
					.WithDegreeOfParallelism(Environment.ProcessorCount)
					.WithExecutionMode(ParallelExecutionMode.ForceParallelism)
					.WithMergeOptions(ParallelMergeOptions.Default)
					.WithCancellation(cts.Token)
					.ForAll(Console.WriteLine);
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine("---");
				Console.WriteLine("Operation has been canceled!");
			}

			Console.WriteLine("---");
			Console.WriteLine("Unordered PLINQ query execution");
			var unorderedQuery = from i in ParallelEnumerable.Range(1, 30)
								 select i;

			foreach (var i in unorderedQuery)
			{
				Console.WriteLine(i);
			}

			Console.WriteLine("---");
			Console.WriteLine("Ordered PLINQ query execution");
			var orderedQuery = from i in ParallelEnumerable.Range(1, 30).AsOrdered()
							   select i;

			foreach (var i in orderedQuery)
			{
				Console.WriteLine(i);
			}

			Console.ReadLine();
		}

		static string EmulateProcessing(string typeName)
		{
			Thread.Sleep(TimeSpan.FromMilliseconds(
				new Random(DateTime.Now.Millisecond).Next(250, 350)));
			Console.WriteLine("{0} type was processed on a thread id {1}",
					typeName, Thread.CurrentThread.ManagedThreadId);
			return typeName;
		}

		static IEnumerable<string> GetTypes()
		{
			return from assembly in AppDomain.CurrentDomain.GetAssemblies()
				   from type in assembly.GetExportedTypes()
				   where type.Name.StartsWith("Web")
				   orderby type.Name.Length
				   select type.Name;
		}
	}
}
