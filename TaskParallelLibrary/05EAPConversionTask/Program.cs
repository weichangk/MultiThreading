using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _05EAPConversionTask
{
	/*
     * .NET1.0 IAsyncResult异步编程模型(APM)”，通过Begin*** 开启操作并返回IAsyncResult对象，使用 End*** 方法来结束操作，通过回调方法来做异步操作后其它事项。然而最大的问题是没有提供进度通知等功能及多线程间控件的访问。为克服这个问题（并解决其他一些问题），
     * .NET2.0 中引入了：基于事件的异步编程模式(EAP，Event-based Asynchronous Pattern)。通过事件、AsyncOperationManager类和AsyncOperation类两个帮助器类实现如下功能：
        1)异步执行耗时的任务。
        2)获得进度报告和增量结果。
        3)支持耗时任务的取消。
        4)获得任务的结果值或异常信息。
        5)更复杂：支持同时执行多个异步操作、进度报告、增量结果、取消操作、返回结果值或异常信息。
        对于相对简单的多线程应用程序，BackgroundWorker组件提供了一个简单的解决方案。对于更复杂的异步应用程序，可以考虑实现一个符合基于事件的异步模式的类。
        有关EAP    https://www.cnblogs.com/heyuquan/archive/2013/04/01/2993085.html
     */


	/*
	 * 这是一个将EAP模式转换为任务的既简单又优美的示例。关键点在于使用TaskCompletionSource<T>类型, T是异步操作结果类型。
	 * 不要忘记将tcs.SekResult调用封装在try-catch代码块中,从而保证错误信息始终会设置给任务完成源对象。也可以使用TrySetResult方法来替代SetResult方法,以保证结果能被成功设置。
	 */
	class Program
	{
		static void Main(string[] args)
		{
			var tcs = new TaskCompletionSource<int>();

			var worker = new BackgroundWorker();
			worker.DoWork += (sender, eventArgs) =>
			{
				eventArgs.Result = TaskMethod("Background worker", 5);
			};

			worker.RunWorkerCompleted += (sender, eventArgs) =>
			{
				if (eventArgs.Error != null)
				{
					tcs.SetException(eventArgs.Error);
				}
				else if (eventArgs.Cancelled)
				{
					tcs.SetCanceled();
				}
				else
				{
					tcs.SetResult((int)eventArgs.Result);
				}
			};

			worker.RunWorkerAsync();

			int result = tcs.Task.Result;

			Console.WriteLine("Result is: {0}", result);

			Console.ReadLine();
		}

		static int TaskMethod(string name, int seconds)
		{
			Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
				name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
			Thread.Sleep(TimeSpan.FromSeconds(seconds));
			return 42 * seconds;
		}
	}
}
