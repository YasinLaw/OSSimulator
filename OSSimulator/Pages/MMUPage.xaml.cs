using OSSimulator.Models.VirtualMemoryPaging;
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
    public sealed partial class MMUPage : Page
    {
        public VM VM { get; set; }

        public int PageCount { get; set; } = 0;

        public int FrameCount { get; set; } = 0;

        public int Mode { get; set; }

        public List<int> Sequence { get; set; }

        public MMUPage()
        {
            this.InitializeComponent();
            VM = new VM();
            Sequence = new List<int>();
        }

        private async void AddFrameButton_Click(object sender, RoutedEventArgs e)
        {
            await AddFrame();
            await AddPage();
        }

        private async Task AddFrame()
        {
            if (FrameCount >= 10)
            {
                return;
            }
            VM.PFrames.Add(new PFrame
            {
                Id = FrameCount++,
                VPage = null
            });
            FramesGridView.ItemsSource = null;
            FramesGridView.ItemsSource = VM.PFrames;
        }

        private async void AddPageButton_Click(object sender, RoutedEventArgs e)
        {
            await AddPage();
        }
        
        private async Task AddPage()
        {
            if (PageCount >= 10)
            {
                return;
            }
            VM.VPages.Add(new VPage
            {
                Id = PageCount++
            });
            PagesGridView.ItemsSource = null;
            PagesGridView.ItemsSource = VM.VPages;
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            await Clear();
            Mode = 0;
        }

        private async void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            await Clear();
            Mode = 1;
        }

        private async void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            await Clear();
            Mode = 2;
        }

        private async Task Clear()
        {
            foreach (var frame in VM.PFrames)
            {
                frame.VPage = null;
            }
            foreach (var page in VM.VPages)
            {
                page.Time = 0;
            }
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            RunButton.IsEnabled = false;
            if (Mode == 0)
            {
                await RunFIFO();
            }
            RunButton.IsEnabled = true;
        }

        private async Task Parse()
        {
            foreach (var s in PagingSequence.Text)
            {
                if (int.TryParse(s.ToString(), out int result))
                {
                    if (result < PageCount)
                    {
                        Sequence.Add(result);
                    }
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        private async Task RunFIFO()
        {
            int count = 0;
            int shot = 0;
            await Parse();
            List<PFrame> frames = new List<PFrame>(VM.PFrames);
            foreach (var s in Sequence)
            {
                count++;
                var page = VM.VPages.FirstOrDefault(x => x.Id == s);
                if (frames.FirstOrDefault(x=>x.VPage == page) != null)
                {
                    var frame = frames.FirstOrDefault(x=>x.VPage == page);
                    frames.Remove(frame);
                    frames.Add(frame);
                    shot++;
                }
                else
                {
                    var frame = frames.FirstOrDefault();
                    frames.Remove(frame);
                    frame.VPage = page;
                    frames.Add(frame);
                }
                Rate.Text = string.Empty;
                Rate.Text += ((int)(shot / (double)count * 100)).ToString() + "%";
                await Task.Delay(1000);
            }
        }
    }
}
