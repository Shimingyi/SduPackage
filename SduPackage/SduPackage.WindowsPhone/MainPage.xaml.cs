using SduPackage.Funcitons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace SduPackage
{

    public sealed partial class MainPage : Page
    {
        Frame rootFrame;
        Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public MainPage()
        {
            rootFrame = Window.Current.Content as Frame;
            
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CheckStatu();
        }

        private void CheckStatu()
        {
            if (_localSettings.Values.ContainsKey("MySummary"))
            {
                if (_localSettings.Values["MySummary"].ToString() == "0.9.0.0")
                {
                    System.Diagnostics.Debug.WriteLine("已经不止一次查看");
                    if (_localSettings.Values.ContainsKey("StuUsername"))
                    {
                        Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            rootFrame.Navigate(typeof(SduPackage.Views.LoadResourcePage));
                            System.Diagnostics.Debug.WriteLine("已经保存了账号");
                        });
                    }
                    else
                    {
                        Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            rootFrame.Navigate(typeof(SduPackage.Views.MyAccount));
                            System.Diagnostics.Debug.WriteLine("没有设置账号");
                        });
                    }
                }
            }
        }

        private void ToMyApp(object sender, RoutedEventArgs e)
        {
            _localSettings.Values["MySummary"] = "0.9.0.0";
            rootFrame.Navigate(typeof(SduPackage.Views.MyAccount));
        }

    }
}
