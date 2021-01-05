using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04ParallelException
{
	class Program
	{
		static void Main(string[] args)
		{
			//IEnumerable<int> numbers = Enumerable.Range(-5, 10);

			//var query = from number in numbers
			//			select 100 / number;

			//try
			//{
			//	foreach (var n in query)
			//		Console.WriteLine(n);
			//}
			//catch (DivideByZeroException)
			//{
			//	Console.WriteLine("Divided by zero!");
			//}

			//Console.WriteLine("---");
			//Console.WriteLine("Sequential LINQ query processing");
			//Console.WriteLine();

			//var parallelQuery = from number in numbers.AsParallel()
			//					select 100 / number; ;

			//try
			//{
			//	parallelQuery.ForAll(Console.WriteLine);
			//}
			//catch (DivideByZeroException)
			//{
			//	Console.WriteLine("Divided by zero - usual exception handler!");
			//}
			//catch (AggregateException e)
			//{
			//	e.Flatten().Handle(ex =>
			//	{
			//		if (ex is DivideByZeroException)
			//		{
			//			Console.WriteLine("Divided by zero - aggregate exception handler!");
			//			return true;
			//		}

			//		return false;
			//	});
			//}

			//Console.WriteLine("---");
			//Console.WriteLine("Parallel LINQ query processing and results merging");

			//Console.ReadLine();

			ExceptionTest();
		}


		static void ExceptionTest()
		{
			try
			{
				var parallelExceptions = new ConcurrentQueue<Exception>();
				Parallel.For(0, 2, (i) =>
				{
					try
					{
						throw new InvalidOperationException("并行任务中出现的异常");
					}
					catch (Exception e)
					{
						parallelExceptions.Enqueue(e);
					}
				});
				if (parallelExceptions.Count > 0)
					throw new AggregateException(parallelExceptions);
			}
			catch (AggregateException err)
			{
				var ae = err.Flatten();
				var exceptions = ae.InnerExceptions;
				Console.WriteLine("Exceptions caught: {0}", exceptions.Count);
				foreach (var e in exceptions)
				{
					//Console.WriteLine("Exception details: {0}", e);
					//Console.WriteLine();
					Console.WriteLine("异常类型：{0}{1}来自：{2}{3}异常内容：{4}", e.GetType(),
					Environment.NewLine, e.Source,
					Environment.NewLine, e.Message);
				}

			}
			Console.WriteLine("主线程马上结束");
			Console.ReadKey();
		}
	}
}
