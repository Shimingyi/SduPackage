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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace SduPackage.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MyAccount : Page
    {
        private Windows.Storage.StorageFolder _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        private Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        int informationMutex, lessionMutex, gradeMutex = 0;

        public MyAccount()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SaveAccount();
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            isLogging.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            LoadAccount();
        }

        private void LogIn(object sender, RoutedEventArgs e)
        {

            isLogging.Visibility = Windows.UI.Xaml.Visibility.Visible;
            var http = new SduPackage.Funcitons.DoPost();
            http.StartPost("http://202.194.14.195:8080/CurriculumServer/login", "Re_Type=Import_Course&user_name=" + LibraryUsername.Text + "&password=" + LibraryPassword.Password, result =>
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
                    catch (IndexOutOfRangeException error)
                    {
                        Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            myNotifitionBar.ShowMessage("账号密码错误");
                            isLogging.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                        });

                    }
                }

            });
            
        }

        private bool CheckResult(string _result)
        {
            bool res = false;
            if (_result == "Error1")
            {
                //NotifitionBar.ShowMessage("请检查网络");
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    myNotifitionBar.ShowMessage("服务器忙，请稍后再试");
                    isLogging.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                });
            }
            if (_result == "Error2")
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    myNotifitionBar.ShowMessage("未知错误");
                    isLogging.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                });
            }
            else
            {
                res = true;
            }
            return res;
        }

        private async void SaveFile(string FileName, string result)
        {
            Windows.Storage.StorageFile tempFile = await _localFolder.CreateFileAsync(FileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(tempFile, result);
            if (FileName == "TheInformationFile.txt")
                informationMutex = 1;
            if (FileName == "TheLessionFile.txt")
                lessionMutex = 1;
            if (FileName == "TheGradeFile.txt")
                gradeMutex = 1;
            if((informationMutex == 1)&&(lessionMutex == 1)&&(gradeMutex==1)){
                _localSettings.Values["hasDownFile"] = "true";
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    isLogging.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    this.Frame.Navigate(typeof(LoadResourcePage));
                });
            }
        }

        #region 事件
        void LoadAccount()
        {
            if (_localSettings.Values.ContainsKey("CardUsername"))
            {
                CardUsername.Text = _localSettings.Values["CardUsername"].ToString();
                CardPassword.Password = _localSettings.Values["CardPassword"].ToString();
                LibraryUsername.Text = _localSettings.Values["StuUsername"].ToString();
                LibraryPassword.Password = _localSettings.Values["StuPassword"].ToString();
            }
        }

        void SaveAccount()
        {
            _localSettings.Values["CardUsername"] = CardUsername.Text;
            _localSettings.Values["CardPassword"] = CardPassword.Password;
            _localSettings.Values["StuUsername"] = LibraryUsername.Text;
            _localSettings.Values["StuPassword"] = LibraryPassword.Password;
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
        #endregion
        
    }
}
