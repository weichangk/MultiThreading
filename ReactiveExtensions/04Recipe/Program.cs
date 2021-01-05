using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _04Recipe
{
	/*
	 * 这里我们展示了创建可观察的对象的不同场景。大部分的功能是通过Observable类型的静态工厂方法实现的。
	 * 前两个例子展示了如何使用值或者不使用值来创建一个Observable方法。接下来的例子中使用Observable.Throw来构造一个Observable类,从而触发其观察者的. OnError处理器。
	 * Observable.Repeat方法代表了一个无尽的序列。该方法有多个重载。这里我们通过重复值42构造了一个无尽的序列。
	 * 然后使用LINQ的Take方法从该序列中提取5个元素。Observable. Range表示了一组值,其与Enumerable.Range非常类似。
	 * 
	 * Observable.Create方法支持更多的自定义场景。有相当多的重载允许我们使用取消标志和任务。但我们先看看最简单的重载。
	 * 它接受一个函数,该函数接受一个观察者实例,并且返回IDisposable对象来代表订阅者。
	 * 如果需要清除任何资源,可以在此放置清除逻辑,但本例中只返回一个空的disposable,因为实际上并不需要。
	 * Observable.Generate是另一个创建自定义序列的方式。我们必须为序列提供一个初始值,然后提供一个断言,来决定是否需要生成更多元素或者完成序列。
	 * 接着提供了一个迭代逻辑,在本例中递增计数器值。最后一个参数是一个选择器函数,允许我们定制化结果。
	 * 
	 * 最后两个方法处理计时器。Observable.Interval会以TimeSpan间隔产生计时器标记事件,Observable. Timer也指定了启动时间。
	 */
	class Program
	{
		static void Main(string[] args)
		{
			IObservable<int> o = Observable.Return(0);
			using (var sub = OutputToConsole(o)) ;
			Console.WriteLine(" ---------------- ");

			o = Observable.Empty<int>();
			using (var sub = OutputToConsole(o)) ;
			Console.WriteLine(" ---------------- ");

			o = Observable.Throw<int>(new Exception());
			using (var sub = OutputToConsole(o)) ;
			Console.WriteLine(" ---------------- ");

			o = Observable.Repeat(42);
			using (var sub = OutputToConsole(o.Take(5))) ;
			Console.WriteLine(" ---------------- ");

			o = Observable.Range(0, 10);
			using (var sub = OutputToConsole(o)) ;
			Console.WriteLine(" ---------------- ");

			o = Observable.Create<int>(ob => {
				for (int i = 0; i < 10; i++)
				{
					ob.OnNext(i);
				}
				return Disposable.Empty;
			});
			using (var sub = OutputToConsole(o)) ;
			Console.WriteLine(" ---------------- ");

			o = Observable.Generate(
				0 // initial state
				, i => i < 5 // while this is true we continue the sequence
				, i => ++i // iteration
				, i => i * 2 // selecting result
			);
			using (var sub = OutputToConsole(o)) ;
			Console.WriteLine(" ---------------- ");

			IObservable<long> ol = Observable.Interval(TimeSpan.FromSeconds(1));
			using (var sub = OutputToConsole(ol))
			{
				Thread.Sleep(TimeSpan.FromSeconds(3));
			};
			Console.WriteLine(" ---------------- ");

			ol = Observable.Timer(DateTimeOffset.Now.AddSeconds(2));
			using (var sub = OutputToConsole(ol))
			{
				Thread.Sleep(TimeSpan.FromSeconds(3));
			};
			Console.WriteLine(" ---------------- ");
		}

		static IDisposable OutputToConsole<T>(IObservable<T> sequence)
		{
			return sequence.Subscribe(
				obj => Console.WriteLine("{0}", obj)
				, ex => Console.WriteLine("Error: {0}", ex.Message)
				, () => Console.WriteLine("Completed")
			);
		}
	}
}
