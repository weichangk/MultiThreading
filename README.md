# C#多线程编程
参考《c#多线程编程实战》学习整理
## 内容列表
- [线程基础](#线程基础)
- [线程同步](#线程同步)
- [使用线程池](#使用线程池)
- [使用任务并行库](#使用任务并行库)
- [C#5.0编程语言中的原生的异步编程方式](#C#5.0编程语言中的原生的异步编程方式)
- [使用并发集合](#使用并发集合)
- [使用PLINQ](#使用PLINQ)
- [使用ReactiveExtensions](#使用ReactiveExtensions)


## 线程基础

#### 01 创建线程
进程是系统进行资源分配和调度的一个独立单位。线程是进程的一个实体，是独立运行和独立调度的基本单位。可以认为线程是一个虚拟的进程。
请记住线程会消耗大量的操作系统资源。多个线程共享一个物理处理器将导致操作系统忙于管理这些线程，而无法运行程序。

创建线程：
static void PrintNumbers()
{
    Console.WriteLine("PrintNumbers Starting...");
    for (int i = 0; i < 10; i++)
    {
        Console.WriteLine(i);
        System.Threading.Thread.Sleep(100);
    }
}
Thread t = new Thread(PrintNumbers);
t.Start();//不会马上的执行

#### 02 暂停线程
暂停线程：
当线程处于休眠状态时,它会占用尽可能少的CPU时间。
Thread.Sleep(TimeSpan.FromSeconds(2));//线程阻塞暂停

#### 03 线程等待
线程等待：
t.Join();//主线程等待子线程终止后继续往下执行；此时主线程处于阻塞状态，实现两线程间的同步执行

#### 04 终止线程
终止线程：
t.Abort();//强制终止线程；这很危险，不推荐使用
这给线程注入了ThreadAbortException方法，导致线程被终结。这非常危险，因为该异常可以在任何时刻发生并可能彻底摧毁应用程序。
另外，使用该技术也不一定总能终止线程。目标线程可以通过处理该异常并调用Thread.ResetAbort方法来拒绝被终止。
因此并不推荐使用Abort方法来关闭线程。可优先使用一些其他方法，比如提供一个CancellationToken方法来取消线程的执行。

#### 05 检测线程状态
检测线程状态：
t.ThreadState.ToString()

#### 06 线程优先级
线程优先级：
线程优先级决定了该线程可占用多少cpu时间
threadOne.Priority = System.Threading.ThreadPriority.Highest;
threadTwo.Priority = System.Threading.ThreadPriority.Lowest;

#### 07 前台线程和后台线程
前台线程和后台线程：
进程会等待所有前台线程都结束后再结束工作，如果只剩下后台程序则直接结束工作。
默认情况下显示创建的线程为前台线程，如果程序定义了一个不会完成的前台线程，主程序并不会正常结束。

#### 08 向线程传参
带参数线程：
var threadTwo = new Thread(Count);//带参线程，线程启动的方法必须接收object类型的单个参数
threadTwo.Start(8);
var threadThree = new Thread(() => CountNumbers(12));//使用lambda表达式可是不用object类型的单个参数；使用lambda表达式引用对象的方式被称为闭包，当表达式中使用任何局部变量，C#会自动生成类，并将该变量作为类的属性。
threadThree.Start();

#### 09 线程使用lock关键字
线程使用lock关键字:
多线程访问同一对象（竞争条件）导致数据不安全。
如果锁定被多线程访问的对象，需要访问该对象的其他线程会处于阻塞状态，并等待直到该对象解除锁定，确保对象访问的安全性，但是导致严重的性能问题。
lock关键字线程锁，隐患：有可能导致死锁。
private readonly object _syncRoot = new Object();//线程锁
lock (_syncRoot)
{
	Count++;
}

#### 10 线程使用Monitor类锁定资源
线程使用Monitor类锁定资源：
lock关键字可能造成死锁。由于死锁将导致程序停止工作，可以使用Monitor类来避免死锁。
实际上lock关键字是Monitor类用例的一个语法糖。
因此，我们可以直接使用Monitor类。其拥有TryEnter方法，该方法接受一个超时参数。如果在我们能够获取被lock保护的资源之前，超时参数过期，则该方法会返回false。
lock (lock2)
{
	Thread.Sleep(1000);
	Console.WriteLine("Monitor.TryEnter allows not to get stuck, returning false after a specified timeout is elapsed");
	if (Monitor.TryEnter(lock1, TimeSpan.FromSeconds(5)))
	{
		Console.WriteLine("Acquired a protected resource succesfully");
	}
	else
	{
		Console.WriteLine("Timeout acquiring a resource!");
	}
}

#### 11 线程处理异常
线程处理异常：
如果是线程启动，不能向上抛出异常，线程外的try/catch也捕获不到线程启动函数异常，向上抛上层也捕获不了还是会程序崩溃。
而是在线程代码中使用try/catch代码块，可以捕获不处理异常避免应用程序强制结束，不建议隐藏异常！

## 线程同步
#### 01 执行基本的原子操作
多个线程同时使用共享对象会造成很多问题。同步这些线程使得对共享对象的操作能够以正确的顺序执行是非常重要的。
Interlocked类，为多线程共享变量提供原子操作，无需用LOCK防止死锁

#### 02 使用Mutex类
Mutex互斥量
当主程序启动时,定义了一个指定名称的互斥量,设置initialOwner标志为false。这意
味着如果互斥量已经被创建,则允许程序获取该互斥量。如果没有获取到互斥量,程序则简单地显示Running,等待直到按下了任何键,然后释放该互斥量并退出。
如果再运行同样一个程序,则会在10秒钟内尝试获取互斥量。如果此时在第一个程序中,按下了任何键,第二个程序则会开始执行。然而,如果保持等待10秒钟,第二个程序将无法获取到该互斥量。
请注意具名的互斥量是全局的操作系统对象!请务必正确关闭互斥量。最好是使用using代码块来包裹互斥量对象。
该方式可用于在不同的程序中同步线程,可被推广到大量的使用场景中。也可以用于检测线程运行超时？

#### 03 使用SemaphoreSlim类
Semaphoreslim类作为Semaphore类的轻量级版本的。该类限制了同时访问同一个资源的线程数量。

#### 04 使用AutoResetEvent类
AutoResetEvent类来从一个线程向另一个线程发送通知。AutoResetEvent类可以通知等待的线程有某事件发生。
当主程序启动时,定义了两个AutoResetEvent实例。其中一个是从子线程向主线程发信号,另一个实例是从主线程向子线程发信号。
我们向AutoResetEvent构造方法传入false,定义了这两个实例的初始状态为unsignaled,这意味着任何线程调用这两个对象中的任何一个的WaitOne方法将会被阻塞,直到我们调用了Set方法。
如果初始事件状态为true,那么AutoResetEvent实例的状态为signaled,如果线程调用WaitOne方法则会被立即处理。
然后事件状态自动变为unsignaled,所以需要再对该实例调用一次Set方法,以便让其他的线程对该实例调用WaitOne方法从而继续执行。
AutoResetEvent类采用的是内核时间模式,所以等待时间不能太长。使用ManualResetEventslim类更好,因为它使用的是混合模式。

#### 05 使用ManualResetEventSlim类
使用ManualResetEventsSlim类来在线程间以更灵活的方式传递信号。
ManualResetEvnetSlim的整个工作方式有点像人群通过大门。AutoResetEvent事件像一个旋转门,一次只允许一人通过。
ManualResetEventSlim是ManualResetEvent的混合版本,一直保持大门敞开直到手动调用Reset方法。当调用_mainEvent.Set时,
相当于打开了大门从而允许准备好的线程接收信号并继续工作。当调用mainEvent.Reset相当于关闭了大门。

#### 06 使用CountDownEvent类
使用CountdownEvent信号类来等待直到一定数量的操作完成。
当主程序启动时,创建了一个CountdownEvent实例,在其构造函数中指定了当两个操作完成时会发出信号。
然后我们启动了两个线程,当它们执行完成后会发出信号。一旦第二个线程完成,主线程会从等待CountdownEvent的状态中返回并继续执行。
针对需要等待多个异步操作完成的情形,使用该方式是非常便利的。
然而这有一个重大的缺点。如果调用_countdown.Signal()没达到指定的次数,那么_countdown. Wait)将一直等待。
请确保使用CountdownEvent时,所有线程完成后都要调用Signal方法。

#### 07 使用Barrier类
Barrier类用于组织多个线程及时，在某个时刻碰面。其提供了一个回调函数,每次线程调用了SignalAndWait方法后该回调函数会被执行。
创建了Barrier类,指定了我们想要同步两个线程。在两个线程中的任何最后一个调用了_barre.signalAndWait方法后,会执行一个回调函数来打印出阶段。
每个线程将向Barrer发送两次信号,所以会有两个阶段。每个阶段的最后一个线程调用SignalAndWait方法时, Barrier将执行回调函数。
这在多线程迭代运算中非常有用,可以在每个迭代结束前执行一些计算。当最后一个线程调用SignalAndWait方法时可以在迭代结束时进行交互。

#### 08 使用ReaderWriterLockSlim类
ReaderWriterLockSlim来创建一个线程安全的机制,在多线程中对一个集合进行读写操作。
ReaderWriterLockSlim代表了一个管理资源访问的锁,允许多个线程同时读取,以及独占写。

#### 09 使用SpinWait类
SpinWait,它是一个混合同步构造,被设计为使用用户模式等待一段时间,然后切换到内核模式以节省CPU 时间。

## 使用线程池
创建线程是昂贵的操作，所以为每个短暂的异步操作创建线程会产生显著的开销。
为了解决该问题，有一个常用的方式叫做池(pooling)。线程池可以成功地适应于任何需要大量短暂的开销大的资源的情形。
我们事先分配一定的资源，将这些资源放入到资源池。每次需要新的资源，只需从池中获取一个，而不用创建一个新的。当该资源不再被使用时,就将其返回到池中。
保持线程中的操作都是短暂的是非常重要的。不要在线程池中放入长时间运行的操作,或者阻塞工作线程。这将导致所有工作线程变得繁忙，从而无法服务用户操作。这会导致性能问题和非常难以调试的错误。
线程池中的工作线程都是后台线程。这意味着当所有的前台线程(包括主程序线程)完成后,所有的后台线程将停止工作。

#### 01 在线程池中调用委托
#### 02 向线程池中放入异步操作
#### 03 线程池与并行度
#### 04 实现一个取消选项
#### 05 在线程池中使用等待事件处理器及超时
#### 06 使用计时器
#### 07 使用Background Worker组件

## 使用任务并行库

.Net Framework4.0引入了一个新的关于异步操作的API。它叫做任务并行库( Task Parallel Library,简称TPL),
TPL可被认为是线程池之上的又一个抽象层,其对程序员隐藏了与线程池交互的底层代码,并提供了更方便的细粒度的API。
TPL的核心概念是任务，一个任务代表了一个异步操作,该操作可以通过多种方式运行,可以使用或不使用独立线程运行。

#### 01 创建任务
#### 02 使用任务执行基本的操作
#### 03 组合任务
#### 04 将APM模式转换为任务
#### 05 将EAP模式转换为任务
#### 06 实现取消选项
#### 07 处理任务中的异常
#### 08 并行运行任务
#### 09 使用TaskScheduler配置任务的执行

## C#5.0编程语言中的原生的异步编程方式
#### 01 使用await操作符获取异步任务结果
#### 02 在lambda表达式中使用await操作符
#### 03 对连续的异步任务使用await操作符
#### 04 对并行执行的异步任务使用await操作符
#### 05 处理异步操作中的异常
#### 06 避免使用捕获的同步上下文
#### 07 使用async void方法
#### 08 设计一个自定义的awaitable类型
#### 09 对动态类型使用await

## 使用并发集合
先从ConcurrentQueue开始。该集合使用了原子的比较和交换(Compare and Swap,简称CAS)操作,以及SpinWait来保证线程安全。
它实现了一个先进先出( First In First Out,简称FIFO)的集合,这意味着元素出队列的顺序与加入队列的顺序是一致的。
可以调用Enqueue方法向队列中加入元素。TryDequeue方法试图取出队列中的第一个元素,而Trypeek方法则试图得到第一个元素但并不从队列中删除该元素。

ConcurrentStack的实现也没有使用任何锁,只采用了CAS操作。它是一个后进先出( Last In First Out,简称LIFO)的集合,这意味着最近添加的元素会先返回。
可以使用Push和PushRange方法添加元素,使用TryPop和TryPopRange方法获取元素,以及使用TryPeek方法检查元素。

ConcurrentBag是一个支持重复元素的无序集合。它针对这样以下情况进行了优化,即多个线程以这样的方式工作:
每个线程产生和消费自己的任务,极少与其他线程的任务交互(如果要交互则使用锁),添加元素使用Add方法,检查元素使用TryPeek方法,获取元素使用TryTake方法。

ConcurrentDictionary是一个线程安全的字典集合的实现。对于读操作无需使用锁。但是对于写操作则需要锁。该并发字典使用多个锁,在字典桶之上实现了一个细粒度的锁模型。
使用参数concurrencyLevel可以在构造函数中定义锁的数量,这意味着预估的线程数量将并发地更新该字典。

BlockingCollection是对IProducerConsumerCollection泛型接口的实现的一个高级封装。它有很多先进的功能来实现管道场景,即当你有一些步骤需要使用之前步骤运行的结果时。
BlockingCollectione类支持如下功能:分块、调整内部集合容量、取消集合操作、从多个块集合中获取元素。
#### 01 使用ConcurrentDictionary
#### 02 使用ConcurrentQueue实现异步处理
#### 03 改变ConcurrentStack异步处理顺序
#### 04 使用ConcurrentBag
#### 05 创建一个可扩展的爬虫
#### 06 使用BlockingCollection进行异步处理

## 使用PLINQ
任务并行库中有一个名为Parallel的类,其提供了一组API用来实现结构并行。
它仍然是TPL的一部分,我们在本章介绍它的原因是它是从较低的抽象层向较高的抽象层过渡的完美例子。
当使用Parallel类的API时,我们无需提供分割工作的细节。但是我们仍要显式定义如何从分割的结果中得到单个结果。

PLINQ具有最高级抽象。它自动将数据分割为数据块,并且决定是否真的需要并行化查·询,或者使用通常的顺序查询处理更高效。
PLINQ基础设施会将分割任务的执行结果组合到一起。有很多选项可供程序员来优化查询,使用尽可能高的性能获取结果。

在本章中我们将涵盖Parallel类的用法以及很多不同的PLINQ选项,例如让LINQ查,询并行化,设置异常模型及设置PLINQ查询的并行等级,处理查询项的顺序,以及处理PLINQ异常。
我们也会学习如何管理PLINQ查询的数据分割
#### 01 使用Parallel类
#### 02 并行化LINQ查询
#### 03 调整PLINQ查询的参数
#### 04 处理PLINQ查询中的异常
#### 05 管理PLINQ查询中的数据分区
#### 06 为PLINQ查询创建一个自定义的聚合器

## 使用ReactiveExtensions
之前我们已经学习过了,有好几种方式在NET和C#中创建异步程序。其中一个是基于事件的异步模式,在之前的章节中已经提及过。
引入事件的初始目的是简化观察者设计模式的实现。该模式常用于实现对象间的通讯。
当我们讨论任务并行库时,我们注意到事件的主要缺点是它们不能有效地相互结合。
另一个缺点是基于事件的异步模式不能处理通知的顺序。想象一下我们有IEnumerable<string>提供给我们字符串值。
然而当迭代它时,不知道每个迭代会花费多长时间。如果使用常规的foreach或其他的同步迭代构造方式,我们将阻塞线程直到得到下一个值,这可能会导致整个处理非常慢。
这种作为客户端从生产者那里拉取值的场景被称为基于拉取(pull-based)的方式。

相反的方式是基于推送(push-based)的方式,即生产者通知客户端有新值要处理。这将把工作推给生产者,而客户端在等待另一个值的时候可以做些其他事情。
因此,目标是实现类似于IEnumerable的异步版本的一个机制,可以生产一组序列值并按顺序通知消费者处理这些值,直到序列处理完成或抛出异常。
NET Framework从4.0版本开始包含了接口1Observablesout T>和IObserverkin T>的定义,它们一起代表了异步的基于推送的集合及其客户端。
它们都来自叫做ReactiveExtensions (简称Rx)的库,其由微软创建,用于使用可观察的集合来有效地构造事件序列,以及实际上任何其他类型的异步程序。
这些接口包括在.Net Framework中,但这些接口的实现类以及所有其他机制仍单独的分布在Rx库中。

Reactive Extensions起初是一个跨平台的库。.NET3.5, Silverlight以及Windows Phone都有相应的库。
甚至JavaScript,Ruby和Python都可以使用Reactive Extensions。
它也是开源的,你可以在CodePlex网站找到针对.NET的Reactive Extensions源码,也可以在GitHub找到其他实现。
#### 01 将普通集合转换为异步的可观察集合
#### 02 编写自定义的可观察对象
#### 03 使用Subject
#### 04 创建可观察的对象
#### 05 对可观察的集合使用LINQ查询
#### 06 使用Rx创建异步操作





