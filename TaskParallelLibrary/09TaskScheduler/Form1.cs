using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _09TaskScheduler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = string.Empty;
            try
            {
                //string result = TaskMethod(TaskScheduler.FromCurrentSynchronizationContext()).Result;
                string result = TaskMethod().Result;
                richTextBox1.Text = result;
            }
            catch (Exception ex)
            {
                richTextBox1.Text = ex.InnerException.Message;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = string.Empty;
            this.Cursor = Cursors.WaitCursor;
            Task<string> task = TaskMethod();
            task.ContinueWith(t => {
                richTextBox1.Text = t.Exception.InnerException.Message;
                this.Cursor = Cursors.Default;
            },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = string.Empty;
            this.Cursor = Cursors.WaitCursor;
            Task<string> task = TaskMethod(TaskScheduler.FromCurrentSynchronizationContext());
            task.ContinueWith(t => this.Cursor = Cursors.Default,
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        Task<string> TaskMethod()
        {
            return TaskMethod(TaskScheduler.Default);
        }

        Task<string> TaskMethod(TaskScheduler scheduler)
        {
            Task delay = Task.Delay(TimeSpan.FromSeconds(5));

            return delay.ContinueWith(t =>
            {
                string str = string.Format("Task is running on a thread id {0}. Is thread pool thread: {1}",
                        Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
                richTextBox1.Text = str;
                return str;
            }, scheduler);
        }
    }
}
