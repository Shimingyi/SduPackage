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
        Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public MyAccount()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            SaveAccount();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            LoadAccount();
        }

        private void LogIn(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoadResourcePage));
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
