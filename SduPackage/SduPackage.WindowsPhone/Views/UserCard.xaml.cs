using SduPackage.Functions;
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
        HttpCookie _loadCookie,_loginCookie;

        public UserCard()
        {
            this.InitializeComponent();
            httpclient = new HttpClient();
            _loadCookie = new HttpCookie("ASP.NET_SessionId", "card.sdu.edu.cn", "/");
            _loginCookie = new HttpCookie("iPlanetDirectoryPro", ".sdu.edu.cn", "/");
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
            try
            {
                string posturl = "http://card.sdu.edu.cn/Account/MiniCheckIn";                
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(posturl));
                request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Name",_loadCookie.Name));
                request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Value", _loadCookie.Value));
                request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Domain", _loadCookie.Domain));
                request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Path", _loadCookie.Path));
                request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Expires", _loadCookie.Expires.ToString()));
                request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Secure", _loadCookie.Secure.ToString()));
                request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("HttpOnly", _loadCookie.HttpOnly.ToString()));

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
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                bool replaced = filter.CookieManager.SetCookie(_loadCookie, false);

                HttpResponseMessage response = await httpclient.SendRequestAsync(request);
                
                //get login cookie
                HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri(posturl));
                foreach (HttpCookie _cookie in cookieCollection)
                {
                    if (_cookie.Name == "iPlanetDirectoryPro")
                    {
                        _loginCookie.Value = _cookie.Value;
                        _loginCookie.Secure = _cookie.Secure;
                        _loginCookie.Expires = _cookie.Expires;
                        _loginCookie.HttpOnly = _cookie.HttpOnly;
                    }
                }
                string responseStr = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseStr);
                if (responseStr.Substring(0, 3) == "suc")
                {
                    string geturl = ("http://card.sdu.edu.cn/CardManage/CardInfo/BasicInfo");
                    HttpRequestMessage second_request = new HttpRequestMessage(HttpMethod.Get, new Uri(geturl));
                    second_request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Name", _loginCookie.Name));
                    second_request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Value", _loginCookie.Value));
                    second_request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Domain", _loginCookie.Domain));
                    second_request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Path", _loginCookie.Path));
                    second_request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Expires", _loginCookie.Expires.ToString()));
                    second_request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Secure", _loginCookie.Secure.ToString()));
                    second_request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("HttpOnly", _loginCookie.HttpOnly.ToString()));
                    HttpResponseMessage second_response = await httpclient.SendRequestAsync(second_request);
                    responseStr = await second_response.Content.ReadAsStringAsync();

                    responseStr = responseStr.Replace(" ", "");
                    System.Diagnostics.Debug.WriteLine("second response:"+responseStr);
                    CardRestMoneyTextBlock.Text = GetRestMoney(responseStr);
                    checkPop.IsOpen = false;
                    appBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    CheckNotifitionBar.ShowMessage(responseStr);
                    ShowImage();
                }
            }
            catch (Exception args)
            {
                System.Diagnostics.Debug.WriteLine(args.ToString());
            }
        }

        private string GetRestMoney(string responseStr)
        {
            string result = string.Empty;
            string[] first_sentences = { "校园卡余额" };
            string[] first_split = responseStr.Split(first_sentences, StringSplitOptions.RemoveEmptyEntries);
            result = first_split[1].Substring(12,5);
            if(result.Substring(4,1)=="<"){
                result = result.Substring(0, 4);
            }
            return result;
        }

        private async void ShowImage()
        {
            //get uri
            TimeSpan ts = DateTime.Now - DateTime.Parse("1970-1-1");
            long tt = Convert.ToInt64(ts.TotalMilliseconds);
            Uri imageUri = new Uri(("http://card.sdu.edu.cn/Account/GetCheckCodeImg/Flag=" + tt.ToString()));
            HttpResponseMessage _response = await httpclient.GetAsync(imageUri);
            IBuffer _buffer = await _response.Content.ReadAsBufferAsync();
            
            //get cookie
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(imageUri);
            foreach (HttpCookie _cookie in cookieCollection)
            {
                _loadCookie.Value = _cookie.Value;
                _loadCookie.Secure = _cookie.Secure;
                _loadCookie.Expires = _cookie.Expires;
                _loadCookie.HttpOnly = _cookie.HttpOnly;
            }

            //get the image
            BitmapImage bi = new BitmapImage();
            await bi.SetSourceAsync(_buffer.AsStream().AsRandomAccessStream());
            Windows.UI.Xaml.Media.ImageSource _imgSource = bi;
            ShowCheckImage.Source = _imgSource;
        }

        

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
