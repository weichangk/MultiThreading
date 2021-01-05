using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace _06ContinueOnCapturedContext
{
	/*
	 * 本节描述了当使用await来获取异步操作结果时,同步上下文行为的细节。我们将学习如何以及何时关闭同步上下文流。
	 * 
	 * 添加引用
	 * WindowsBase
	 * resentationCore
	 * PresentationFramework
	 * 
	 * 在本例中,我们将学习异步函数默认行为的最重要的方面之一。我们已经从第4章中了解了任务调度程序和同步上下文。
	 * 默认情况下, await操作符会尝试捕获同步上下文,并在其中执行代码。我们已经知道这有助于我们编写与用户界面控制器协作的异步代码。
	 * 另外,使用await不会发生在之前章节中描述过的死锁情况,因为当等待结果时并不会阻塞U1线程。
	 * 
	 * 这是合理的,但是让我们看看潜在会发生什么事。在本例中,我们使用编程方式创建了一个Windows Presentation Foundation应用程序并订阅了它的按钮点击事件。
	 * 当点击该按钮时,运行了两个异步操作。其中一个使用了一个常规的await操作符,另一个使用了带false参数值的ConfigureAwait方法。
	 * false参数明确指出我们不能对其使用捕获的同步上下文来运行后续操作代码。在每个操作中,我们测量了执行完成花费的时间,然后将各自的时间和比例显示在主屏幕上。
	 * 结果看到常规的await操作符花费了更多的时间来完成。这是因为我们向U1线程中放入了成百上千个后续操作任务,这会使用它的消息循环来异步地执行这些任务。
	 * 在本例中,我们无需在U1线程中运行该代码,因为异步操作并未访问U1组件。使用带false参数值的ConfigureAwait方法是一个更高效的方案。
	 * 
	 * 公平起见,让我们来看看相反的情况。在前面的代码片段中,在Click方法中,取消注释的代码行,并注释掉紧挨着它的前一行代码。
	 * 当运行程序时,我们将得到多线程控制访问异常,因为设置Label控制器文本的代码不会放置到捕捉的上下文中,而是在线程池的工作线程中执行。
	 */
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			var app = new Application();
			var win = new Window();
			var panel = new StackPanel();
			var button = new Button();
			_label = new Label();
			_label.FontSize = 32;
			_label.Height = 200;
			button.Height = 100;
			button.FontSize = 32;
			button.Content = new TextBlock { Text = "Start asynchronous operations" };
			button.Click += Click;
			panel.Children.Add(_label);
			panel.Children.Add(button);
			win.Content = panel;
			app.Run(win);

			Console.ReadLine();
		}

		async static void Click(object sender, EventArgs e)
		{
			_label.Content = new TextBlock { Text = "Calculating..." };
			TimeSpan resultWithContext = await Test();
			TimeSpan resultNoContext = await TestNoContext();
			//TimeSpan resultNoContext = await TestNoContext().ConfigureAwait(false);
			var sb = new StringBuilder();
			sb.AppendLine(string.Format("With the context: {0}", resultWithContext));
			sb.AppendLine(string.Format("Without the context: {0}", resultNoContext));
			sb.AppendLine(string.Format("Ratio: {0:0.00}",
				resultWithContext.TotalMilliseconds / resultNoContext.TotalMilliseconds));
			_label.Content = new TextBlock { Text = sb.ToString() };
		}

		async static Task<TimeSpan> Test()
		{
			const int iterationsNumber = 10000;
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < iterationsNumber; i++)
			{
				var t = Task.Run(() => { });
				await t;
			}
			sw.Stop();
			return sw.Elapsed;
		}

		async static Task<TimeSpan> TestNoContext()
		{
			const int iterationsNumber = 10000;
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < iterationsNumber; i++)
			{
				var t = Task.Run(() => { });
				await t.ConfigureAwait(
					continueOnCapturedContext: false);
			}
			sw.Stop();
			return sw.Elapsed;
		}

		private static Label _label;
	}
}
