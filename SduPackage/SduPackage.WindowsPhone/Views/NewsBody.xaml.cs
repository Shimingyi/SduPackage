using SduPackage.Funcitons;
using SduPackage.Model;
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
using Windows.Web.Http;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace SduPackage.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewsBody : Page
    {
        string newsSource;

        public NewsBody()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            News _news = e.Parameter as News;
            newsSource = _news.source;
            NewsTitleTextBlock.Text = _news.title;
            NewsReporterTextBlock.Text = ("记者： " + _news.reporter);
            NewsTimeTextBlock.Text = _news.editTime;
            LoadNewsBody(_news.blockId,_news.pid);
        }

        private async void ViewNewsInIE(object sender, RoutedEventArgs e)
        {
            string uriToLaunch = newsSource;
            var uri = new Uri(uriToLaunch);
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }



        private void LoadNewsBody(int block,int pid)
        {
            if(block == 11){
                var http = new DoPost();
                http.StartPost("http://www.online.sdu.edu.cn/News2s/servlet/OtherPassageContentServlet", ("pid=" + pid), result =>
                {
                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        isDowning.IsActive = false;
                        NewsBodyWebView.NavigateToString("<html><body>"+result.Substring(2,result.Length-2)+"</body></html>");
                    });
                });
            }
            //学线通知
            else
            {
                var http = new DoPost();
                http.StartPost("http://www.online.sdu.edu.cn/News2s/servlet/PassageContentServlet", ("pid=" + pid), result =>
                {
                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        //NewsBodyTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        isDowning.IsActive = false;
                        NewsBodyWebView.NavigateToString("<html><body>" + result.Substring(2, result.Length - 2) + "</body></html>");
                        //NewsBodyTextBlock.Text = result;
                    });
                });
            }            
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
        
    }
}
