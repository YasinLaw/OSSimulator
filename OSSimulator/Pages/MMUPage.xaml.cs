using OSSimulator.Models.VirtualMemoryPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        public decimal Count { get; set; } = 0;

        public decimal Shot { get; set; } = 0;

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
                Id = PageCount++,
                Hex = string.Empty 
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
                page.Hex = string.Empty;
            }
            Count = 0;
            Shot = 0;
            Rate.Text = string.Empty;
            Rate.Text += "0%";
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            Count = -FrameCount;
            RunButton.IsEnabled = false;
            AddFrameButton.IsEnabled = false;
            AddPageButton.IsEnabled = false;
            if (Mode == 0)
            {
                await RunFIFO();
            }
            if (Mode == 1)
            {
                await RunLRU();
            }
            if (Mode == 2)
            {
                await RunOPT();
            }
            RunButton.IsEnabled = true;
            AddFrameButton.IsEnabled = true;
            AddPageButton.IsEnabled = true;
        }

        private async Task Parse()
        {
            Sequence.Clear();
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

        private async Task RefreshRate()
        {
            if (Count < 1)
            {
                return;
            }
            Rate.Text = string.Empty;
            decimal result =  Math.Round(Shot / Count * 100);
            Rate.Text += result.ToString() + "%";
        }

        private async Task RunFIFO()
        {
            await Parse();
            List<PFrame> frames = new List<PFrame>(VM.PFrames);
            foreach (var s in Sequence)
            {
                Count++;
                var page = VM.VPages.FirstOrDefault(x => x.Id == s);
                var frame = frames.FirstOrDefault(x => x.VPage == page);
                if (frame != null)
                {
                    Shot++;
                }
                else
                {
                    frame = frames.FirstOrDefault();
                    if (frame.VPage != null)
                    {
                        frame.VPage.Hex = string.Empty;
                    }
                    frame.VPage = page;
                    page.Hex = string.Format("0x100{0:X}", frame.Id);
                    frames.Remove(frame);
                    frames.Add(frame);
                }
                await RefreshRate();
                await Task.Delay(1000);
            }
        }

        private async Task RunLRU()
        {
            await Parse();
            List<PFrame> frames = new List<PFrame>(VM.PFrames);
            foreach (var s in Sequence)
            {
                Count++;
                var page = VM.VPages.FirstOrDefault(x => x.Id == s);
                if (frames.FirstOrDefault(x => x.VPage == page) != null)
                {
                    var frame = frames.FirstOrDefault(x => x.VPage == page);
                    frames.Remove(frame);
                    frames.Add(frame);
                    Shot++;
                }
                else
                {
                    var frame = frames.FirstOrDefault();
                    frames.Remove(frame);
                    if (frame.VPage != null)
                    {
                        frame.VPage.Hex = string.Empty;
                    }
                    frame.VPage = page;
                    page.Hex = string.Format("0x100{0:X}", frame.Id);
                    frames.Add(frame);
                }
                await RefreshRate();
                await Task.Delay(1000);
            }
        }

        private async Task RunOPT()
        {
            await Parse();
            List<PFrame> frames = new List<PFrame>(VM.PFrames);
            List<int> sequence = new List<int>(Sequence);
            foreach (var s in Sequence)
            {
                Count++;
                sequence.RemoveAt(0);
                var page = VM.VPages.FirstOrDefault(x => x.Id == s);
                if (frames.FirstOrDefault(x => x.VPage == page) != null)
                {
                    Shot++;
                }
                else
                {
                    var frame = frames.FirstOrDefault(x => x.VPage == null);
                    if (frame != null)
                    {
                        frame.VPage = page;
                        page.Hex = string.Format("0x100{0:X}", frame.Id);
                    }
                    else
                    {
                        // 帧满时，需要置换在未来序列中出现最少的页
                        List<int> ls = new List<int>();
                        foreach (var f in frames)
                        {
                            ls.Add(f.VPage.Id);
                        }
                        List<int> frequency = new List<int>();
                        frequency.AddRange(ls);
                        foreach (var seq in sequence)
                        {
                            if (ls.Contains(seq))
                            {
                                frequency.Add(seq);
                            }
                        }
                        var pageId = frequency.GroupBy(x => x).OrderBy(grp => grp.Count()).FirstOrDefault().Key;
                        frame = frames.FirstOrDefault(x => x.VPage.Id == pageId);
                        if (frame.VPage != null)
                        {
                            frame.VPage.Hex = string.Empty;
                        }
                        frame.VPage = page;
                        page.Hex = string.Format("0x100{0:X}", frame.Id);
                    }
                }
                await RefreshRate();
                await Task.Delay(1000);
            }
        }
    }
}
