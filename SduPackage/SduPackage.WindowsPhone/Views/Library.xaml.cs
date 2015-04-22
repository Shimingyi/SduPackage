using SduPackage.ViewModel;
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
    
    public sealed partial class Library : Page
    {

        BookViewModel _bookViewModel;
        Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        
        public Library()
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
            LoadBook();
        }

        #region 页面事件
        
        #endregion

        #region 方法
        public void LoadBook()
        {
            Change_StatuBar("正在登录......",0);
            checkAccount();
            _bookViewModel = new BookViewModel(1);
          
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

        #region 私有方法
        private void checkAccount()
        {
            var http = new SduPackage.Funcitons.DoPost();
            string username = _localSettings.Values["CardUsername"].ToString();
            string password = _localSettings.Values["CardPassword"].ToString();
            string url = ("http://202.194.14.195:8080/curriculumlib/lib");
            string postContent = string.Format("requesttype=0&username={0}&password={1}", username, password);
            http.StartPost(url, postContent, result =>
            {
                System.Diagnostics.Debug.WriteLine(result);
                
                if (result.IndexOf("错误")>0)
                {
                    Change_StatuBar("登录失败，请检查你的账号密码", 0);
                }
                else
                {
                    Change_StatuBar("口袋山大", 0);
                }
            });
        }

        private async void Change_StatuBar(string str, double process)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Windows.UI.ViewManagement.StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = (Resources["ButtonPressedBackgroundThemeBrush"] as SolidColorBrush).Color;
                statusBar.ForegroundColor = Windows.UI.Colors.White;
                statusBar.BackgroundOpacity = 1;
                statusBar.ProgressIndicator.Text = str;
                statusBar.ProgressIndicator.ProgressValue = process;
                statusBar.ProgressIndicator.ShowAsync();
            });
            
        }
        #endregion
    }
}
