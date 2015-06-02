using SduPackage.Funcitons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace SduPackage.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoadResourcePage : Page
    {

        private Windows.Storage.StorageFolder _localFolder;
        Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public LoadResourcePage()
        {
            this.InitializeComponent();
            this._localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            downToFile();
            /*
            ThreadPoolTimer.CreateTimer(async t =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    isDowning.IsActive = false;
                    Frame.Navigate(typeof(Views.Index));
                });
            }, TimeSpan.FromSeconds(2));
             */
        }

        private void downToFile()
        {
            string CardUsername = _localSettings.Values["CardUsername"].ToString();
            string CardPassword = _localSettings.Values["CardPassword"].ToString();
            string StuUsername = _localSettings.Values["StuUsername"].ToString();
            string StuUserPassword = _localSettings.Values["StuPassword"].ToString();

            var http = new DoPost();
            //学线通知
            http.StartPost("http://www.online.sdu.edu.cn/News2s/servlet/NewestListServlet", "", result =>
            {
                if (CheckResult(result))
                {
                    result = result.Substring(2, result.Length - 2);
                    SaveFile("TheNewsFromSduOnline.txt", result);
                }
            });
            //其他通知
            http.StartPost("http://www.online.sdu.edu.cn/News2s/servlet/OtherListServlet", "id=11&page=1", result =>
            {

                if (CheckResult(result))
                {
                    result = result.Substring(2, result.Length - 2);
                    SaveFile("TheNewsFromOthers.txt", result);
                }
            });
            //个人信息
            http.StartPost("http://202.194.14.195:8080/CurriculumServer/login", "Re_Type=Import_Course&user_name=" + StuUsername + "&password=" + StuUserPassword, result =>
            {
                if (CheckResult(result))
                {
                    result = result.Substring(2, result.Length - 2);
                    string[] stringSeparators = new string[] { "学生在线课程格子" };
                    string[] temp = result.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    try
                    {
                        SaveFile("TheInformationFile.txt", temp[0]);
                        SaveFile("TheLessionFile.txt", temp[1]);
                        SaveFile("TheGradeFile.txt", result);
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            NotifitionBar.ShowMessage("网络状况不好");
                        });

                    }
                }
                
            });
        }

        private async void SaveFile(string FileName, string result)
        {
            Windows.Storage.StorageFile tempFile = await _localFolder.CreateFileAsync(FileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(tempFile, result);
            if (FileName == "TheGradeFile.txt")
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Frame.Navigate(typeof(Views.Index));
                });
            }
        }

        private bool CheckResult(string _result)
        {
            bool res = false;
            if (_result == "Error1")
            {
                //NotifitionBar.ShowMessage("请检查网络");
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    isDownBlock.Text = "网络状况不好，加载失败";
                    isDowningBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    againButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                });
            }
            if (_result == "Error2")
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    isDownBlock.Text = "加载失败";
                    isDowningBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    againButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                });
            }
            else{
                res = true;
            }
            return res;
        }

        private void DownAgain(object sender, RoutedEventArgs e)
        {
            isDownBlock.Text = "Loading";
            isDowning();
            downToFile();
        }

        private void notDowning()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                isDowningBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                againButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            });
        }

        private void isDowning()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                isDowningBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                isDownBlock.Text = "Loading";
                againButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            });
        }



    }
}