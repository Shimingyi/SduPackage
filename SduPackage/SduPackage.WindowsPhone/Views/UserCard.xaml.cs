using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace SduPackage.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserCard : Page
    {
        Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        Windows.Web.Http.Filters.HttpBaseProtocolFilter filter;
        HttpClient httpclient;

        public UserCard()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadPage();
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
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
            else
            {
                myPopup.IsOpen = false;
            }
        }

        private void LoadPage()
        {
            //isDowning.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //CardLogin();
            CheckBankNum();
            ShowCheckPop();
            ShowImage();
        }

        #region 页面事件
        private void CheckCheckNum(object sender, RoutedEventArgs e)
        {
            string checkCode = CheckTextBox.Text;
            CardLogin(checkCode);
        }

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

        #endregion
       

        private async void CardLogin(string _checkCode)
        {            
            //开始登录
            string username = _localSettings.Values["CardUsername"].ToString();
            string password = _localSettings.Values["CardPassword"].ToString();
            filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            httpclient = new HttpClient(filter);
            try
            {

                string posturl = "http://card.sdu.edu.cn/Account/MiniCheckIn";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(posturl));
                HttpFormUrlEncodedContent postDate = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("signtype","SynCard"),
                        new KeyValuePair<string,string>("username",username),
                        new KeyValuePair<string,string>("password",password),
                        new KeyValuePair<string,string>("checkcode",_checkCode),
                        new KeyValuePair<string,string>("isUsedKeyPad","false")
                    }
                );
                request.Content = postDate;
                HttpResponseMessage response = await httpclient.SendRequestAsync(request);
                string responseStr = await response.Content.ReadAsStringAsync();
                if (responseStr.Substring(0, 7) == "success")
                {
                    httpclient = new HttpClient(filter);
                    responseStr = await httpclient.GetStringAsync(new Uri("http://card.sdu.edu.cn/CardManage/CardInfo/BasicInfo"));
                    responseStr = responseStr.Replace(" ", "");
                    int index = responseStr.IndexOf("校园卡余额");
                    System.Diagnostics.Debug.WriteLine(responseStr.Length);
                    string result = responseStr.Substring(index,index+5);
                    System.Diagnostics.Debug.WriteLine(result);
                }
                else
                {
                    
                }

            }
            catch (Exception args)
            {
                System.Diagnostics.Debug.WriteLine(args.ToString());
            }
        }

        

        #region 状态改变
        private void ShowImage()
        {
            TimeSpan ts = DateTime.Now - DateTime.Parse("1970-1-1");
            long tt = Convert.ToInt64(ts.TotalMilliseconds);
            Uri imageUri = new Uri(("http://card.sdu.edu.cn/Account/GetCheckCodeImg/Flag=" + tt.ToString()));
            Windows.UI.Xaml.Media.ImageSource _imgSource = new BitmapImage(imageUri);
            ShowCheckImage.Source = _imgSource;
        }

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

        private void ShowCheckPop()
        {
            checkGrid.Height = Window.Current.Bounds.Height;
            checkGrid.Width = Window.Current.Bounds.Width;
            checkPop.IsOpen = true;
            appBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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
