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
        private Windows.Storage.StorageFolder _localFolder;

        public MainPage()
        {
            this.InitializeComponent();
            this._localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.Index));
            downToFile();
            ThreadPoolTimer.CreateTimer(async t =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    isDowning.IsActive = false;
                    Frame.Navigate(typeof(Views.Index));
                });

            }, TimeSpan.FromSeconds(2));
        }

        private void downToFile()
        {
            var http = new DoPost();
            //学线通知
            http.StartPost("http://www.online.sdu.edu.cn/News2s/servlet/NewestListServlet", "", result =>
            {
                result = result.Substring(2, result.Length - 2);
                SaveFile("TheNewsFromSduOnline.txt", result);
            });
            //其他通知
            http.StartPost("http://www.online.sdu.edu.cn/News2s/servlet/OtherListServlet", "id=11&page=1", result =>
            {
                result = result.Substring(2, result.Length - 2);
                SaveFile("TheNewsFromOthers.txt", result);
            });
            
        }

        private async void SaveFile(string FileName, string result)
        {
            Windows.Storage.StorageFile tempFile = await _localFolder.CreateFileAsync(FileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(tempFile, result);
        }

    }
}
