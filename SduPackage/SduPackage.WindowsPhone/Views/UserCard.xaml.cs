using SduPackage.Functions;
using SduPackage.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace SduPackage.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserCard : Page
    {
        Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        HttpClient httpclient;
        CardViewModel _cardViewModel = new CardViewModel();

        public UserCard()
        {
            this.InitializeComponent();
            httpclient = new HttpClient();
            _cardViewModel = new CardViewModel();
            LoadPage();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (!myPopup.IsOpen)
            {
                if(this.Frame.CanGoBack){
                    this.Frame.GoBack();
                }
            }
            else
            {
                myPopup.IsOpen = false;
                appBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        private void LoadPage()
        {
            CheckBankNum();
            _cardViewModel.LoadCard(isDowning, myNotifitionBar, CardRestMoneyTextBlock);
        }

        #region 页面事件
        

        private void BindingBankButton(object sender, RoutedEventArgs e)
        {
            if (!myPopup.IsOpen)
            {
                ShowBankNumPop();
            }
        }

        private void FinishBankNum(object sender, RoutedEventArgs e)
        {
            string bankNum = BankNumTextBox.Text;
            if (bankNum.Length == 19)
            {
                _localSettings.Values["BankCardNum"] = bankNum;
                myPopup.IsOpen = false;
                BankCardLastFour.Text = bankNum.Substring((bankNum.Length - 4));
                NoBankNum.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ShowBankNum.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        private void ToMyCardRecord(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MyCardHistory), _cardViewModel);
        }

        private void ToRecharge(object sender, RoutedEventArgs e)
        {
            myNotifitionBar.ShowMessage("调试中，下个版本推出，敬请期待 0.0");
        }

        private void ToRefresh(object sender, RoutedEventArgs e)
        {
            isDowning.Visibility = Windows.UI.Xaml.Visibility.Visible;
            _cardViewModel.LoadCard(isDowning, myNotifitionBar, CardRestMoneyTextBlock);
        }
        #endregion


        #region 状态改变
        

        private void CheckBankNum()
        {
            if (_localSettings.Values.ContainsKey("BankCardNum"))
            {
                NoBankNum.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                string bankNum = _localSettings.Values["BankCardNum"].ToString();
                BankCardLastFour.Text = bankNum.Substring((bankNum.Length - 4));
                ShowBankNum.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        private void ShowBankNumPop()
        {
            test.Height = Window.Current.Bounds.Height;
            test.Width = Window.Current.Bounds.Width;
            myPopup.IsOpen = true;
            appBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            if (_localSettings.Values.ContainsKey("BankCardNum"))
            {
                BankNumTextBox.Text = _localSettings.Values["BankCardNum"].ToString();
            }
        }
        #endregion

       

    }
}
