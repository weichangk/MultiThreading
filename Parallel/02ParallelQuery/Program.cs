using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02ParallelQuery
{
	/*
	 * 当程序运行时,我们创建了一个LINQ查询,其使用反射API来查询加载到当前应用程序域中的所有组件中名称以"Web"开头的类型。
	 * 我们使用EmulateProcessing方法模拟处理每个项时间的延迟,并使用PrintInfo方法打印结果。我们也使用了Stopwatch类来测量每个查询的执行时间。
	 * 
	 * 首先我们运行了一个通常的顺序LINQ查询。此时并没有并行化,所有任何操作都运行在当前线程。该查询的第二版显式地使用了ParallelEnumerable类。
	 * ParallelEnumerable包含了PLINQ的逻辑实现,并且作为IEnumerable集合功能的一组扩展方法。通常无需显式地使用该类,在这里是为了演示PLINQ的实际工作方式。
	 * 第二个版本以并行的方式运行EmulateProcessing操作。然而,默认情况下结果会被合并到单个线程中,所以查询的执行时间应该比第一个版本少几秒。
	 * 
	 * 第三个版本展示了如何使用AsParalle方法来将LINQ查询按声明的方式并行化运行。这里我们并不关心实现细节,只是为了说明我们想以并行的方式运行。
	 * 然而,该版本的关键不同处是我们使用了ForAll方法来打印查询结果。打印结果操作与任务被处理的线程是同个线程,跳过了结果合并步骤。
	 * 它允许我们也能以并行的方式运行PrintInfo方法,甚至该版本运行速度比之前的版本更快。
	 * 
	 * 最后一个例子展示了如何使用AsSequential方法将PLINQ查询以顺序方式运行。可以,看到该查询运行方式与第一个示例完全一样。
	 */
	class Program
	{
		static void Main(string[] args)
		{
			var sw = new Stopwatch();
			sw.Start();
			var query = from t in GetTypes()
						select EmulateProcessing(t);

			foreach (string typeName in query)
			{
				PrintInfo(typeName);
			}
			sw.Stop();
			Console.WriteLine("---");
			Console.WriteLine("Sequential LINQ query.");
			Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
			Console.WriteLine("Press ENTER to continue....");
			Console.ReadLine();
			Console.Clear();
			sw.Reset();

			sw.Start();
			var parallelQuery = from t in ParallelEnumerable.AsParallel(GetTypes())
								select EmulateProcessing(t);

			foreach (var typeName in parallelQuery)
			{
				PrintInfo(typeName);
			}
			sw.Stop();
			Console.WriteLine("---");
			Console.WriteLine("Parallel LINQ query. The results are being merged on a single thread");
			Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
			Console.WriteLine("Press ENTER to continue....");
			Console.ReadLine();
			Console.Clear();
			sw.Reset();

			sw.Start();
			parallelQuery = from t in GetTypes().AsParallel()
							select EmulateProcessing(t);

			parallelQuery.ForAll(PrintInfo);

			sw.Stop();
			Console.WriteLine("---");
			Console.WriteLine("Parallel LINQ query. The results are being processed in parallel");
			Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
			Console.WriteLine("Press ENTER to continue....");
			Console.ReadLine();
			Console.Clear();
			sw.Reset();

			sw.Start();
			query = from t in GetTypes().AsParallel().AsSequential()
					select EmulateProcessing(t);

			foreach (string typeName in query)
			{
				PrintInfo(typeName);
			}

			sw.Stop();
			Console.WriteLine("---");
			Console.WriteLine("Parallel LINQ query, transformed into sequential.");
			Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
			Console.WriteLine("Press ENTER to continue....");
			Console.ReadLine();
			Console.Clear();
		}

		static void PrintInfo(string typeName)
		{
			Thread.Sleep(TimeSpan.FromMilliseconds(150));
			Console.WriteLine("{0} type was printed on a thread id {1}",
					typeName, Thread.CurrentThread.ManagedThreadId);
		}

		static string EmulateProcessing(string typeName)
		{
			Thread.Sleep(TimeSpan.FromMilliseconds(150));
			Console.WriteLine("{0} type was processed on a thread id {1}",
					typeName, Thread.CurrentThread.ManagedThreadId);
			return typeName;
		}

		static IEnumerable<string> GetTypes()
		{
			return from assembly in AppDomain.CurrentDomain.GetAssemblies()
				   from type in assembly.GetExportedTypes()
				   where type.Name.StartsWith("Web")
				   select type.Name;

		}
	}
}
