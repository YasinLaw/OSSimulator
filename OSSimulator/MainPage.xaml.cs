using OSSimulator.Pages;
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
using System.Threading.Tasks;

namespace OSSimulator
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Star_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/YasinLaw/OSSimulator"));
        }

        private async void Mail_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("mailto:YasinLaw@outlook.com"));
        }

        private void Nvview_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            FrameNavigationOptions options = new FrameNavigationOptions
            {
                TransitionInfoOverride = args.RecommendedNavigationTransitionInfo,
                IsNavigationStackEnabled = false
            };
            Type pageType = null;
            var itemContainer = args.InvokedItemContainer;
            if (args.IsSettingsInvoked)
            {
                pageType = typeof(SettingsPage);
            }
            else if (itemContainer == PS)
            {
                pageType = typeof(PSPage);
            }
            else if (itemContainer == MMU)
            {
                pageType = typeof(MMUPage);
            }
            contentFrame.NavigateToType(pageType, null, options);
        }
    }
}
