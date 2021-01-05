﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _08UseReaderWriterLockSlim
{
	/*
	ReaderWriterLockSlim来创建一个线程安全的机制,在多线程中对一个集合进行读写操作。
	ReaderWriterLockSlim代表了一个管理资源访问的锁,允许多个线程同时读取,以及独占写。
	*/

	class Program
    {
        static void Main(string[] args)
        {
			new Thread(Read) { IsBackground = true }.Start();
			new Thread(Read) { IsBackground = true }.Start();
			new Thread(Read) { IsBackground = true }.Start();

			new Thread(() => Write("Thread 1")) { IsBackground = true }.Start();
			new Thread(() => Write("Thread 2")) { IsBackground = true }.Start();

			Thread.Sleep(TimeSpan.FromSeconds(30));
		}

		static ReaderWriterLockSlim _rw = new ReaderWriterLockSlim();
		static Dictionary<int, int> _items = new Dictionary<int, int>();

		static void Read()
		{
			Console.WriteLine("Reading contents of a dictionary");
			while (true)
			{
				try
				{
					_rw.EnterReadLock();
					foreach (var key in _items)
					{
						Thread.Sleep(TimeSpan.FromSeconds(1));
						Console.WriteLine("key value {0}", key.Value);
					}
				}
				finally
				{
					_rw.ExitReadLock();
				}
			}
		}

		static void Write(string threadName)
		{
			while (true)
			{
				try
				{
					int newKey = new Random().Next(3);
					_rw.EnterUpgradeableReadLock();
					if (!_items.ContainsKey(newKey))
					{
						try
						{
							_rw.EnterWriteLock();
							_items[newKey] = newKey;
							Console.WriteLine("New key {0} is added to a dictionary by a {1}", newKey, threadName);
						}
						finally
						{
							_rw.ExitWriteLock();
						}
					}
					Thread.Sleep(TimeSpan.FromSeconds(0.1));
				}
				finally
				{
					_rw.ExitUpgradeableReadLock();
				}
			}
		}
	}

}
