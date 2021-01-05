using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _05RegisterWaitForSingleObject
{
	/*
	 * 线程池还有一个有用的方法: ThreadPool.RegiserWaitForsingleobjecto该方法允许我们·将回调函数放入线程池中的队列中。
	 * 当提供的等待事件处理器收到信号或发生超时时,该回调函数将被调用。这允许我们为线程池中的操作实现超时功能。
	 */
	class Program
    {
		static void Main(string[] args)
		{
			RunOperations(TimeSpan.FromSeconds(3));
			//RunOperations(TimeSpan.FromSeconds(7));

			Console.ReadLine();
		}

		static void RunOperations(TimeSpan workerOperationTimeout)
		{
			using (var evt = new ManualResetEvent(false))
			using (var cts = new CancellationTokenSource())
			{
				Console.WriteLine("Registering timeout operations...");
				var worker = ThreadPool.RegisterWaitForSingleObject(evt,
					(state, isTimedOut) => WorkerOperationWait(cts, isTimedOut), null, workerOperationTimeout, true);

				Console.WriteLine("Starting long running operation...");

				ThreadPool.QueueUserWorkItem(_ => WorkerOperation(cts.Token, evt));

				Thread.Sleep(workerOperationTimeout.Add(TimeSpan.FromSeconds(2)));//+2秒 为了防止WorkerOperationWait调用了using释放后的CancellationTokenSource。该阻塞时间尽量大于超时时间
				worker.Unregister(evt);
			}
		}

		static void WorkerOperation(CancellationToken token, ManualResetEvent evt)
		{
			for (int i = 0; i < 6; i++)
			{
				if (token.IsCancellationRequested)
				{
					return;
				}
				Thread.Sleep(TimeSpan.FromSeconds(1));
				Console.WriteLine(i);
			}
			evt.Set();
		}

		static void WorkerOperationWait(CancellationTokenSource cts, bool isTimedOut)
		{
			if (isTimedOut)
			{
				cts.Cancel();
				Console.WriteLine("Worker operation timed out and was canceled.");
			}
			else
			{
				Console.WriteLine("Worker operation succeded.");
			}
		}
		/*
		 * 当从ManualResetEvent对象接受到一个信号后,该异步操作会被调用。如果操作顺利完成,会设置该信号量。
		 * 另一种情况是操作还未完成就已经超时。如果发生了该情况,我们会使用CancellationToken来取消操作。
		 */
	}
}
