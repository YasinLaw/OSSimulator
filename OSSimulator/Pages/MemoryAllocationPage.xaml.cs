using OSSimulator.Models.MemoryAllocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace OSSimulator.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MemoryAllocationPage : Page
    {
        public List<Memory> Memories { get; set; } = new List<Memory>(32);

        public List<DoTask> DoTasks { get; set; } = new List<DoTask>(6);

        public string TasksInfo { get; set; } = string.Empty;

        public MemoryAllocationPage()
        {
            this.InitializeComponent();
            for (int i = 0; i < 32; i++)
            {
                Memories.Add(new Memory
                {
                    Task = null
                });
            }
            Random random = new Random();
            MemoryGrid.ItemsSource = null;
            MemoryGrid.ItemsSource = Memories;
            DoTasks.Add(new DoTask
            {
                Id = 0,
                Length = random.Next(1, 18),
                Color = "Red"
            });
            DoTasks.Add(new DoTask
            {
                Id = 1,
                Length = random.Next(1, 18),
                Color = "Green"
            });
            DoTasks.Add(new DoTask
            {
                Id = 2,
                Length = random.Next(1, 18),
                Color = "DarkBlue"
            });
            DoTasks.Add(new DoTask
            {
                Id = 3,
                Length = random.Next(1, 18),
                Color = "LightBlue"
            });
            DoTasks.Add(new DoTask
            {
                Id = 4,
                Length = random.Next(1, 18),
                Color = "YellowGreen"
            });
            DoTasks.Add(new DoTask
            {
                Id = 5,
                Length = random.Next(1, 18),
                Color = "DarkGray"
            });

            foreach (var t in DoTasks)
            {
                TasksInfo += $"TaskId: {t.Id}\tLength: {t.Length}\n";
            }
            
            Tasks.Text = string.Empty;
            Tasks.Text = TasksInfo;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DoTasks[0].MemoryId != -1)
            {
                return;
            }
            await Swap(DoTasks[0]);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (DoTasks[1].MemoryId != -1)
            {
                return;
            }
            await Swap(DoTasks[1]);
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (DoTasks[2].MemoryId != -1)
            {
                return;
            }
            await Swap(DoTasks[2]);
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (DoTasks[3].MemoryId != -1)
            {
                return;
            }
            await Swap(DoTasks[3]);
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (DoTasks[4].MemoryId != -1)
            {
                return;
            }
            await Swap(DoTasks[4]);
        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (DoTasks[5].MemoryId != -1)
            {
                return;
            }
            await Swap(DoTasks[5]);
        }

        private async Task Swap(DoTask task)
        {
            string op = string.Empty;
            var index = -1;
            op += "Removed: ";
            while (index == -1)
            {
                var tuple = await FindFreeUnits(task);
                await RemoveTask(tuple.Item2);
                op += tuple.Item2 == null ? "null " : tuple.Item2.Id.ToString() + " ";
                index = tuple.Item1;
            }
            op += "Launched: " + task.Id;
            Op.Text = op;
           await LaunchTask(task, index);
        }

        /// <summary>
        /// Find a space which launching task fits most.
        /// </summary>
        /// <param name="task"></param>
        /// <returns>Index of free units and removing task. If no enough space, return index -1.</returns>
        private async Task<Tuple<int, DoTask>> FindFreeUnits(DoTask task)
        {
            // Have enough free units.
            var index = -1;
            var count = 0;
            for (int i = 0; i < 32; i++)
            {
                if (!Memories[i].Occupied)
                {
                    count++;
                    index = i + 1;
                    if (count == task.Length)
                    {
                        return new Tuple<int, DoTask>(index - count, null);
                    }
                }
                else
                {
                    count = 0;
                }
            }

            // Need to swap.
            index = -1;
            List<DoTask> tasks = new List<DoTask>();
            foreach (var m in Memories)
            {
                if (m.Task != null && !tasks.Contains(m.Task))
                {
                    tasks.Add(m.Task);
                }
            }

            // Tuple<index, length, removingtask>.
            List<Tuple<int, int, DoTask>> tuples = new List<Tuple<int, int, DoTask>>();
            foreach (var t in tasks)
            {
                var i = t.MemoryId;
                var len = 0;
                for (; i > -1 && !Memories[i].Occupied; i--) ;
                for (int tmp = i; tmp < 32 && (!Memories[tmp].Occupied || Memories[tmp].Task == t); tmp++)
                {
                    len++;
                }
                tuples.Add(new Tuple<int, int, DoTask>(i, len, t));
            }

            var rt = (DoTask)null;
            var ordered = tuples.OrderBy(x => x.Item2);
            var order = ordered.FirstOrDefault(x => x.Item2 >= task.Length);
            if (order != null)
            {
                index = order.Item1;
                rt = order.Item3;
            }
            else
            {
                rt = ordered.FirstOrDefault().Item3;
            }
            return new Tuple<int, DoTask>(index, rt);
        }
        
        private async Task LaunchTask(DoTask task, int index)
        {
            var range = Memories.GetRange(index, task.Length);
            foreach (var m in range)
            {
                m.Color = task.Color;
                m.Occupied = true;
                m.Task = task;
            }
            task.MemoryId = index;
            task.TaskState = DoTask.State.INSTALLED;
        }

        private async Task RemoveTask(DoTask task)
        {
            if (task == null)
            {
                return;
            }
            var range = Memories.GetRange(Memories.IndexOf(Memories.FirstOrDefault(x=>x.Task == task)), task.Length);
            foreach (var m in range)
            {
                m.Occupied = false;
                m.Color = "Transparent";
                m.Task = null;
            }
            task.MemoryId = -1;
            task.TaskState = DoTask.State.UNINSTALLED;
        }
    
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var lens = AllocMemory.Text.Split(' ');
            List<int> ls = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                int.TryParse(lens[i], out int res);
                ls.Add(res);
            }
            Tasks.Text = string.Empty;
            TasksInfo = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                DoTasks[i].Length = ls[i];
                TasksInfo += $"TaskId: {DoTasks[i].Id}\tLength: {DoTasks[i].Length}\n";
            }
            Tasks.Text = TasksInfo;
        }
    }
}
