using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01EnumerableToObservable
{
	/*
	 我们使用EnumerableEventSequence方法模拟了一个效率不高的可枚举的集合。
	 然后使用常用的foreach循环来迭代它,可以看到这一如既往的慢。等待直到所有迭代完成。
	 然后借助于Reactive Extensions库中的ToObservable扩展方法把可枚举的集合转换为可观察的集合。
	 接下来订阅该可观察集合的更新,提供Console Write方法作为操作,其将在每次更新该集合时被执行。
	 结果我们得到了与上一个例子完全一样的行为。
	 我们会等待所有迭代完成,因为我们使用了主线程来订阅这些更新。

	 为了异步化该程序,我们使用Subscribeon方法并提供其TPL任务池调度程序。该调度程序将把订阅信息放置到TPL任务池中,卸除主线程的任务。
	 这可以使U1在集合更新时仍保持响应并做些其他事情。为了检查该行为,你可以从代码中除移最后的Consol.ReadLine调用。
	 这样主线程会立即完成,从而强制所有的后台线程(包括TPL任务池工作线程)也一起结束,我们将不会从异步集合中得到任何输出。
	 */
	class Program
    {
		static void Main(string[] args)
		{
			//foreach (int i in EnumerableEventSequence())
			//{
			//	Console.WriteLine(i);
			//}
			//Console.WriteLine("IEnumerable");

			IObservable<int> o = EnumerableEventSequence().ToObservable();
			using (IDisposable subscription = o.Subscribe(Console.WriteLine))
			{
				Console.WriteLine("IObservable");
			}

			o = EnumerableEventSequence().ToObservable().SubscribeOn(TaskPoolScheduler.Default);
			using (IDisposable subscription = o.Subscribe(Console.WriteLine))
			{
				Console.WriteLine("IObservable async");
				//Console.ReadLine();
				for (int i = 100; i < 110; i++)
				{
					Thread.Sleep(TimeSpan.FromSeconds(1));
					Console.WriteLine(i);
				}
			}

			//Console.ReadLine();
		}

		static IEnumerable<int> EnumerableEventSequence()
		{
			for (int i = 0; i < 10; i++)
			{
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
				yield return i;
			}
		}
	}
}
