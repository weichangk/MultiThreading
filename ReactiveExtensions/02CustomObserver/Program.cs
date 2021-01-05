using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02CustomObserver
{
	/*
	 这里我们先实现了定义的观察者,简单地向控制台打印可观察集合中下一项的相关信息、错误信息以及序列完成信息。
	 这是非常简单的消费者代码,没什么特别的。有意思的部分是可观察的集合实现。
	 我们在构造函数中接受一个数字枚举,故意不检查它是否为null,然后创建一个订阅的观察者,我们迭代该集合并通知观察者枚举集合中的每个项。

	 然后我们演示了该实际订阅行为。正如我们所看到的,通过SubscribeOn方法实现了异步。
	 该方法是10bservable的一个扩展方法,并且包含异步订阅逻辑。我们不关心如何实现可观察的集合中的异步,而是用Reactive Extensions库中的标准实现。
	 当订阅平常的可观察集合时,我们只是从中得到了其所有元素。现在它是异步的,所以需要等待一些时间来让异步操作完成,然后打印信息并等待用户输人。
	 最后我们尝试订阅下一个可观察的集合,我们迭代一个空的枚举因此得到了一个空引用异常。
	 可以看到该异常被正确的处理,并且OnError方法被执行,打印出了错误细节。
	 */
	class Program
    {
		static void Main(string[] args)
		{
			var observer = new CustomObserver();

			var goodObservable = new CustomSequence(new[] { 1, 2, 3, 4, 5 });
			var badObservable = new CustomSequence(null);

			using (IDisposable subscription = goodObservable.Subscribe(observer))
			{
			}

			using (IDisposable subscription = goodObservable.SubscribeOn(TaskPoolScheduler.Default).Subscribe(observer))
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(100));
				Console.WriteLine("Press ENTER to continue");
				Console.ReadLine();
			}

			using (IDisposable subscription = badObservable.SubscribeOn(TaskPoolScheduler.Default).Subscribe(observer))
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(100));
				Console.WriteLine("Press ENTER to continue");
				Console.ReadLine();
			}
		}

		class CustomObserver : IObserver<int>
		{
			public void OnNext(int value)
			{
				Console.WriteLine("Next value: {0}; Thread Id: {1}", value, Thread.CurrentThread.ManagedThreadId);
			}

			public void OnError(Exception error)
			{
				Console.WriteLine("Error: {0}", error.Message);
			}

			public void OnCompleted()
			{
				Console.WriteLine("Completed");
			}
		}

		class CustomSequence : IObservable<int>
		{
			private readonly IEnumerable<int> _numbers;

			public CustomSequence(IEnumerable<int> numbers)
			{
				_numbers = numbers;
			}
			public IDisposable Subscribe(IObserver<int> observer)
			{
				foreach (var number in _numbers)
				{
					observer.OnNext(number);
				}
				observer.OnCompleted();
				return Disposable.Empty;
			}
		}
	}
}
