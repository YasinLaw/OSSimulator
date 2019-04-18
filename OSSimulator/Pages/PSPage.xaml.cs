using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OSSimulator.Models.ProcessSchedule;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace OSSimulator.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PSPage : Page
    {
        Windows.Storage.StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;

        Windows.Storage.StorageFile file;

        public string Log { get; set; }

        public readonly object mutex = new object();

        public ThreadCollection ThreadCollection { get; set; }

        public bool IsPriority { get; set; } = true;

        public int Count { get; set; } = 0;

        public Random Random { get; set; } = new Random();

        public ThreadModel Current { get; set; }

        public PSPage()
        {
            this.InitializeComponent();
            ThreadCollection = new ThreadCollection();
            PrCk.IsChecked = true;
        }

        private async Task Clear()
        {
            foreach (var thread in ThreadCollection.Threads)
            {
                thread.Value = 0;
                thread.ProcState = ThreadModel.State.READY;
                thread.Priority = thread.AllocPriority;
            }
            ThreadCollection.RunningThreads.AddRange(ThreadCollection.Threads);
            ThreadCollection.BlockedThreads.Clear();
        }

        private async void Priority_Checked(object sender, RoutedEventArgs e)
        {
            IsPriority = true;
            await Clear();
        }

        private async void RoundRobin_Checked(object sender, RoutedEventArgs e)
        {
            IsPriority = false;
            await Clear();
        }

        

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var thread = new ThreadModel
            {
                Pid = Count++,
                AllocPriority = Random.Next(100),
                AllocTime = Random.Next(100),
                ProcState = ThreadModel.State.READY
            };
            thread.Priority = thread.AllocPriority;
            ThreadCollection.RunningThreads.Add(thread);
            ThreadCollection.Threads.Add(thread);
            ThreadsDataGrid.ItemsSource = null;
            ThreadsDataGrid.ItemsSource = ThreadCollection.Threads;
            ProgressBars.ItemsSource = null;
            ProgressBars.ItemsSource = ThreadCollection.Threads;
        }

        private async void DelButton_Click(object sender, RoutedEventArgs e)
        {
            var thread = ThreadCollection.RunningThreads.FirstOrDefault();
            ThreadCollection.RunningThreads.Remove(thread);
            ThreadCollection.BlockedThreads.Remove(thread);
            ThreadCollection.Threads.Remove(thread);
            ThreadsDataGrid.ItemsSource = null;
            ThreadsDataGrid.ItemsSource = ThreadCollection.Threads;
            ProgressBars.ItemsSource = null;
            ProgressBars.ItemsSource = ThreadCollection.Threads;
            if (ThreadCollection.Threads.Count == 0)
            {
                Count = 0;
            }
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            file = await folder.CreateFileAsync("log.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);
            RunButton.IsEnabled = false;
            AddButton.IsEnabled = false;
            DelButton.IsEnabled = false;
            if (ThreadCollection.BlockedThreads.Count != 0)
            {
                ThreadCollection.RunningThreads.AddRange(ThreadCollection.BlockedThreads);
                ThreadCollection.BlockedThreads.Clear();
            }
            foreach (var thread in ThreadCollection.RunningThreads)
            {
                thread.ProcState = ThreadModel.State.READY;
                thread.IsRunning = true;
            }
            if (IsPriority)
            {
                await RunPrio();
            }
            else
            {
                await RunRR();
            }
            AddButton.IsEnabled = true;
            DelButton.IsEnabled = true;
            RunButton.IsEnabled = true;
        }

        private async Task RunPrio()
        {
            await LogOnce();
            while (ThreadCollection.RunningThreads.Count != 0)
            {
                lock (mutex)
                {
                    ThreadCollection.RunningThreads.Sort();
                    Current = ThreadCollection.RunningThreads.FirstOrDefault();
                    Current.ProcState = ThreadModel.State.RUNNING;
                    Current.IsRunning = true;
                }
                await Task.Delay(100);
                lock (mutex)
                {
                    if (Current.ProcState == ThreadModel.State.BLOCKED)
                    {
                        continue;
                    }
                    Current.Priority -= 3;
                    Current.Value++;
                    if (Current.Value == Current.AllocTime)
                    {
                        Current.ProcState = ThreadModel.State.FINISHED;
                        ThreadCollection.RunningThreads.Remove(Current);
                        Current.IsRunning = false;
                        var item = ProgressBars.Items;
                        continue;
                    }
                    Current.ProcState = ThreadModel.State.READY;
                }
                await LogOnce();
            }
            await Logging();
        }

        private async Task BlockThread()
        {
            lock (mutex)
            {
                if (Current.ProcState == ThreadModel.State.RUNNING)
                {
                    Current.ProcState = ThreadModel.State.BLOCKED;
                    ThreadCollection.RunningThreads.Remove(Current);
                    ThreadCollection.BlockedThreads.Add(Current);
                    Current.IsRunning = false;
                }
            }
        }

        private async Task RunRR()
        {
            await LogOnce();
            while (ThreadCollection.RunningThreads.Count != 0)
            {
                lock (mutex)
                {
                    Current = ThreadCollection.RunningThreads.FirstOrDefault();
                    Current.ProcState = ThreadModel.State.RUNNING;
                    Current.IsRunning = true;
                }
                await Task.Delay(100);
                lock (mutex)
                {
                    if (Current.ProcState == ThreadModel.State.BLOCKED)
                    {
                        continue;
                    }
                    Current.Value++;
                    ThreadCollection.RunningThreads.Remove(Current);
                    if (Current.Value == Current.AllocTime)
                    {
                        Current.ProcState = ThreadModel.State.FINISHED;
                        Current.IsRunning = false;
                        continue;
                    }
                    Current.ProcState = ThreadModel.State.READY;
                    ThreadCollection.RunningThreads.Add(Current);
                }
                await LogOnce();
            }
            await Logging();
        }

        private async void BlockButton_Click(object sender, RoutedEventArgs e)
        {
            await BlockThread();
        }

        private async Task ClearCache()
        {
            Windows.Storage.StorageFile file = await folder.CreateFileAsync("log.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);
            if (file.IsAvailable)
            {
                await file.DeleteAsync();
            }
            Log = string.Empty;
        }

        private async Task Logging()
        {
            if (file.IsAvailable)
            {
                await Windows.Storage.FileIO.AppendTextAsync(file, Log);
            }
            else
            {
                text.Text = "Wrong";
            }
        }

        private async Task LogOnce()
        {
            foreach (var t in ThreadCollection.Threads)
            {
                Log += $"Pid：{t.Pid}\t\t分配优先值：{t.Priority}\t\t优先值：{t.Priority}\t\t分配时间：{t.AllocTime}\t\t运行时间：{t.Value}\t\t进程状态：{t.ProcState}\n";
            }
            Log += "--------------------------------------------------------------------------------------------------------------------\n";
        }

        private async void ClearCache_Click(object sender, RoutedEventArgs e)
        {
            await ClearCache();
        }
    }
}
