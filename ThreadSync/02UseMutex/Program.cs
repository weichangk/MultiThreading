using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02UseMutex
{
	/*
	Mutex互斥量

	当主程序启动时,定义了一个指定名称的互斥量,设置initialOwner标志为false。这意
	味着如果互斥量已经被创建,则允许程序获取该互斥量。如果没有获取到互斥量,程序则简单地显示Running,等待直到按下了任何键,然后释放该互斥量并退出。
	如果再运行同样一个程序,则会在10秒钟内尝试获取互斥量。如果此时在第一个程序中,按下了任何键,第二个程序则会开始执行。然而,如果保持等待10秒钟,第二个程序将无法获取到该互斥量。
	请注意具名的互斥量是全局的操作系统对象!请务必正确关闭互斥量。最好是使用using代码块来包裹互斥量对象。
	该方式可用于在不同的程序中同步线程,可被推广到大量的使用场景中。也可以用于检测线程运行超时？
	*/
	class Program
    {
        static void Main(string[] args)
        {
			const string MutexName = "CSharpThreadingCookbook";

			using (var m = new Mutex(false, MutexName))
			{
				if (!m.WaitOne(TimeSpan.FromSeconds(10), false))
				{
					Console.WriteLine("Second instance is running!");
				}
				else
				{
					Console.WriteLine("Running!");
					Console.ReadLine();
					m.ReleaseMutex();
				}
			}
			Console.ReadLine();

		}
    }
}
