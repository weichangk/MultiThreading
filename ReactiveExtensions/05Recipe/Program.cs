using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _05Recipe
{
	/*
	 * 对可观察的事件序列使用LINQ的能力是Reactive Extensions框架的主要优势。其,实有很多不同的使用场景。
	 * 遗憾的是在这里无法展示全部。我尝试提供一个非常简单的说明示例,这里没有非常复杂的细节,展示了LINQ查询应用到异步的可观察的集合的精华。
	 * 
	 * 首先我们创建了Observable事件来创建一个数字序列,每个数字用时50毫秒。我们从初始值0开始,产生21个这样的事件。然后将LINQ查询结合到该序列中。
	 * 首先,我们只选择序列中的奇数,然后再只选择偶数,最后再连接这两个序列。最后的查询展示了如何使用非常有用的方法Do,其允许引入副作用(side effect)。
	 * 比如,从结果序列中记录每个值。为了运行所有的查询,我们创建了嵌套的订阅。由于该序列初始时是异步的,所以要非常小心订阅的生命周期。
	 * 外部范围代表了向计时器的订阅,内部订阅则各自处理组合的序列查询和副作用查询。如果按下回车键太早,我们只是停止对计时器的订阅,这会导致停止该示例运行。
	 * 
	 * 当运行该示例时,我们可以看到不同的查询实时交互的实际过程。可以看到这些查询是惰性的,只有我们订阅了其结果,查询才会运行。计时器事件序列打印在第一列。
	 * 当事件号查询得到一个事件号时,也会打印出该号,并使用---做前缀来区分第一列的序列结果。最,终查询结果打印在右边的列。当程序运行时,可以看到计时器序列、奇数序列和副作用序列并行的运行。
	 * 只有连接操作会等待直到奇数序列完成。如果我们不连接这些序列,我们将有4个并行序列事件以最高效的方式相互交互!这展示了Reactive Extensions的真正力量,是深入学习该库的好的开端。
	 */
	class Program
	{
		static void Main(string[] args)
		{
			IObservable<long> sequence = Observable.Interval(TimeSpan.FromMilliseconds(50)).Take(21);

			var evenNumbers = from n in sequence
							  where n % 2 == 0
							  select n;

			var oddNumbers = from n in sequence
							 where n % 2 != 0
							 select n;

			var combine = from n in evenNumbers.Concat(oddNumbers)
						  select n;

			var nums = (from n in combine
						where n % 5 == 0
						select n)
					.Do(n => Console.WriteLine("------Number {0} is processed in Do method", n));

			using (var sub = OutputToConsole(sequence, 0))
			using (var sub2 = OutputToConsole(combine, 1))
			using (var sub3 = OutputToConsole(nums, 2))
			{
				Console.WriteLine("Press enter to finish the demo");
				Console.ReadLine();
			}
		}

		static IDisposable OutputToConsole<T>(IObservable<T> sequence, int innerLevel)
		{
			string delimiter = innerLevel == 0 ? string.Empty : new string('-', innerLevel * 3);
			return sequence.Subscribe(
				obj => Console.WriteLine("{0}{1}", delimiter, obj)
				, ex => Console.WriteLine("Error: {0}", ex.Message)
				, () => Console.WriteLine("{0}Completed", delimiter)
			);
		}
	}
}
