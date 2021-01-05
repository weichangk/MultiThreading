using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _03Subject
{
	/*
	 在本程序中我们遍历了Subject类型家族的不同变种。Subiect代表了1Observable和1observer这两个接口的实现。
	 在不同的代理场景中它非常有用,比如将事件从多个源中转换,到一个流中,反过来也一样,广播事件序列给多个订阅者。Subject与Reactive Extensions起使用也非常方便。

	 先从基本的Subject类型开始。一旦订阅了Subject,它就会把事件序列发送给订阅者。在本例中,字符串A不会被打印,因为订阅发生在传播之后。
	 基于此,当对Observable调用OnCompleted和OnError方法时,它停止了进一步的事件序列传播,所以最后的字符串也不能打印出来。

	 接下来的类型ReplaySubject是非常灵活的,允许我们实现三个附加的场景。首先,从广播开始可以缓存所有的事件,如果稍后订阅,将先获得之前所有的事件。
	 第二个例子演示该行为。这里控制台输出了4个字符串,因为第一个事件将被缓存并传送给稍后的订阅者。

	 接着我们指定了ReplaySubject的缓存大小和时间窗口大小。接下来的例子中,我们设置该subject的缓存只能容纳2个事件。如果广播了更多的事件,只有最后两个会被再次传送给订阅者。
	 所以我们看不到第一个字符串,因为当订阅它时只有B和C在subject的缓存中。使用时间窗口的行为也类似。
	 我们可以指定subject只缓存发生在确定时间以内的事件,丢弃掉旧数据。因此,第四个例子中,我们只看到了最后两个事件。较老的事件并不符合时间窗口限制。

	 AsyncSubject类似于任务并行库中的Task类型。它代表了单个异步操作。如果有多个事件发布,它将等待事件序列完成,并把最后一个事件提供给订阅者。
	 BehaviorSubject与ReplaySubject类型很相似,但它只缓存一个值,并允许万一还没有发送任何通知时,指定一个默认值。
	 在最后一个例子中,可以看到打印出了所有字符串,因为我们提供了默认值,所有其他事件都已经发生在订阅之后。
	 如果向上移动behaviorSubject.OnNext"B");代码行到Default事件下面,其将替换掉输出中的默认值。
	 */
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Subject");
			var subject = new Subject<string>();

			subject.OnNext("A");
			using (var subscription = OutputToConsole(subject))
			{
				subject.OnNext("B");
				subject.OnNext("C");
				subject.OnNext("D");
				subject.OnCompleted();
				subject.OnNext("Will not be printed out");
			}

			Console.WriteLine("ReplaySubject");
			var replaySubject = new ReplaySubject<string>();

			replaySubject.OnNext("A");
			using (var subscription = OutputToConsole(replaySubject))
			{
				replaySubject.OnNext("B");
				replaySubject.OnNext("C");
				replaySubject.OnNext("D");
				replaySubject.OnCompleted();
			}

			Console.WriteLine("Buffered ReplaySubject");
			var bufferedSubject = new ReplaySubject<string>(2);

			bufferedSubject.OnNext("A");
			bufferedSubject.OnNext("B");
			bufferedSubject.OnNext("C");
			using (var subscription = OutputToConsole(bufferedSubject))
			{
				bufferedSubject.OnNext("D");
				bufferedSubject.OnCompleted();
			}

			Console.WriteLine("Time window ReplaySubject");
			var timeSubject = new ReplaySubject<string>(TimeSpan.FromMilliseconds(200));

			timeSubject.OnNext("A");
			Thread.Sleep(TimeSpan.FromMilliseconds(100));
			timeSubject.OnNext("B");
			Thread.Sleep(TimeSpan.FromMilliseconds(100));
			timeSubject.OnNext("C");
			Thread.Sleep(TimeSpan.FromMilliseconds(100));
			using (var subscription = OutputToConsole(timeSubject))
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(300));
				timeSubject.OnNext("D");
				timeSubject.OnCompleted();
			}

			Console.WriteLine("AsyncSubject");
			var asyncSubject = new AsyncSubject<string>();

			asyncSubject.OnNext("A");
			using (var subscription = OutputToConsole(asyncSubject))
			{
				asyncSubject.OnNext("B");
				asyncSubject.OnNext("C");
				asyncSubject.OnNext("D");
				asyncSubject.OnCompleted();
			}

			Console.WriteLine("BehaviorSubject");
			var behaviorSubject = new BehaviorSubject<string>("Default");
			using (var subscription = OutputToConsole(behaviorSubject))
			{
				behaviorSubject.OnNext("B");
				behaviorSubject.OnNext("C");
				behaviorSubject.OnNext("D");
				behaviorSubject.OnCompleted();
			}

			Console.ReadLine();
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
