using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _07BackgroundWorker
{
    class Program
    {
		/*
		 * 本节实例演示了另一种异步编程的方式,即使用BackgroundWorker组件。
		 * 借助于该对象,可以将异步代码组织为一系列事件及事件处理器。使用该组件进行异步编程。
		 * BackgroundWorker组件的实例。显式地指出该后台工作线程支持取消操作及该操作进度的通知。
		 * 利用BackgroundWorker组件三种事件：DoWork，ProgressChanged，RunWorkerCompleted
		 */
		static void Main(string[] args)
		{
			using (var bw = new BackgroundWorker())
			{
				bw.WorkerReportsProgress = true;
				bw.WorkerSupportsCancellation = true;

				bw.DoWork += Worker_DoWork;
				bw.ProgressChanged += Worker_ProgressChanged;
				bw.RunWorkerCompleted += Worker_Completed;

				bw.RunWorkerAsync();

				Console.WriteLine("Press C to cancel work");
				do
				{
					if (Console.ReadKey(true).KeyChar == 'C')
					{
						bw.CancelAsync();
					}

				}
				while (bw.IsBusy);
			}

		}

		static void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			Console.WriteLine("DoWork thread pool thread id: {0}", Thread.CurrentThread.ManagedThreadId);
			var bw = (BackgroundWorker)sender;
			for (int i = 1; i <= 100; i++)
			{

				if (bw.CancellationPending)
				{
					e.Cancel = true;
					return;
				}

				if (i % 10 == 0)
				{
					bw.ReportProgress(i);
				}

				Thread.Sleep(TimeSpan.FromSeconds(0.1));
			}
			e.Result = 42;
		}

		static void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			Console.WriteLine("{0}% completed. Progress thread pool thread id: {1}", e.ProgressPercentage,
				Thread.CurrentThread.ManagedThreadId);
		}

		static void Worker_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			Console.WriteLine("Completed thread pool thread id: {0}", Thread.CurrentThread.ManagedThreadId);
			if (e.Error != null)
			{
				Console.WriteLine("Exception {0} has occured.", e.Error.Message);
			}
			else if (e.Cancelled)
			{
				Console.WriteLine("Operation has been canceled.");
			}
			else
			{
				Console.WriteLine("The answer is: {0}", e.Result);
			}
		}
	}
}
