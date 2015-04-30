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
            //个人信息
            http.StartPost("http://202.194.14.195:8080/CurriculumServer/login12", "Re_Type=Import_Course&user_name=201300301197&password=641806", result =>
            {
                result = result.Substring(2, result.Length - 2);
                string[] stringSeparators = new string[] { "学生在线课程格子" };
                string[] temp = result.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                SaveFile("TheInformationFile.txt",temp[1]);
                SaveFile("TheCourseFile.txt", temp[2]);
                SaveFile("TheGradeFile.txt", temp[3]);
            });
        }

        private async void SaveFile(string FileName, string result)
        {
            Windows.Storage.StorageFile tempFile = await _localFolder.CreateFileAsync(FileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(tempFile, result);
        }

    }
}
