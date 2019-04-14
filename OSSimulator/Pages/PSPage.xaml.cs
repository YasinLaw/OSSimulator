using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using OSSimulator.Models.ProcessSchedule;
using System.Collections.Concurrent;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace OSSimulator.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PSPage : Page
    {
        public ThreadCollection ThreadCollection { get; set; }

        public bool IsPriority { get; set; }

        public int Count { get; set; } = 0;

        public Random Random { get; set; } = new Random();

        public PSPage()
        {
            this.InitializeComponent();
            ThreadCollection = new ThreadCollection();
        }

        private void Priority_Checked(object sender, RoutedEventArgs e)
        {
            IsPriority = true;
        }

        private void RoundRobin_Checked(object sender, RoutedEventArgs e)
        {
            IsPriority = false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadCollection.Threads.TryAdd(new ThreadModel
            {
                Pid = Count++,
                Time = Random.Next(1000),
                Priority = Random.Next(1000),
                ProcState = ThreadModel.State.READY,
            });
        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadCollection.Threads.TryTake(out ThreadModel thread);
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BlockButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
