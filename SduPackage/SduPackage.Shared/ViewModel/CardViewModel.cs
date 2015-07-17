using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SduPackage.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.UI.Xaml.Controls;
using Windows.Storage.Streams;
using Windows.Web.Http.Filters;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using SduPackage.Controls;


namespace SduPackage.ViewModel
{
    public class CardViewModel : INotifyPropertyChanged
    {
        #region 字段
        Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        string checkCode;
        HttpClient httpClient;
        public HttpCookie _loadCookie, _loginCookie;
        public string accountRest;
        #endregion

        #region 属性
        public ObservableCollection<CardRecord> RecordSet
        {
            get;
            private set;
        }
        #endregion

        #region 事件
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region 构造方法
        public CardViewModel()
        {
            httpClient = new HttpClient();
            _loadCookie = new HttpCookie("ASP.NET_SessionId", "card.sdu.edu.cn", "/");
            _loginCookie = new HttpCookie("iPlanetDirectoryPro", ".sdu.edu.cn", "/");
            this.RecordSet = new ObservableCollection<CardRecord>();
            this.RaisePropertyChanged("RecordSet");
        }
        #endregion

        #region 方法
        public async void LoadCard(Grid _isdowning, NotificationBar _notification, TextBlock CardRestMoneyTextBlock)
        {
            await GetLoginCheckImage();
            await GetLoginHtml(_isdowning, _notification, CardRestMoneyTextBlock);
        }

        public async void LoadRecord(ProgressRing _isdowning)
        {
            await GetRecordHtml(_isdowning);
        }
        #endregion

        #region 私有方法
        private async Task<string> GetLoginCheckImage()
        {
            //get uri
            TimeSpan ts = DateTime.Now - DateTime.Parse("1970-1-1");
            long tt = Convert.ToInt64(ts.TotalMilliseconds);
            Uri imageUri = new Uri(("http://card.sdu.edu.cn/Account/GetCheckCodeImg/Flag=" + tt.ToString()));
            HttpResponseMessage _response = await httpClient.GetAsync(imageUri);
            Windows.Storage.Streams.IBuffer _buffer = await _response.Content.ReadAsBufferAsync();

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
            WriteableBitmap bitmap = new WriteableBitmap(60, 20);
            await bitmap.SetSourceAsync(_buffer.AsStream().AsRandomAccessStream());
            checkCode = SduPackage.Functions.IdentifyCheckCode.Identify(bitmap);
            System.Diagnostics.Debug.WriteLine(checkCode);
            return checkCode;
        }

        private async Task<string> GetLoginHtml(Grid _isdowning, NotificationBar CheckNotifitionBar, TextBlock CardRestMoneyTextBlock)
        {
            string username = _localSettings.Values["CardUsername"].ToString();
            string password = _localSettings.Values["CardPassword"].ToString();
            try
            {
                string posturl = "http://card.sdu.edu.cn/Account/MiniCheckIn";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(posturl));
                request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Name", _loadCookie.Name));
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
                        new KeyValuePair<string,string>("checkcode",checkCode),
                        new KeyValuePair<string,string>("isUsedKeyPad","false")
                    }
                );
                request.Content = postDate;
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                bool replaced = filter.CookieManager.SetCookie(_loadCookie, false);

                HttpResponseMessage response = await httpClient.SendRequestAsync(request);

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
                    HttpResponseMessage second_response = await httpClient.SendRequestAsync(second_request);
                    responseStr = await second_response.Content.ReadAsStringAsync();

                    responseStr = responseStr.Replace(" ", "");
                    CheckNotifitionBar.ShowMessage("登陆成功");
                    _isdowning.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    accountRest = GetRestMoney(responseStr);
                    CardRestMoneyTextBlock.Text = accountRest;

                    return "0";
                }
                else
                {
                    CheckNotifitionBar.ShowMessage(responseStr);
                    _isdowning.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    return "1";
                }
            }
            catch (Exception args)
            {
                _isdowning.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                return "2";
            }
        }

        private async Task<string> GetRecordHtml(ProgressRing _isdowning)
        {
            string result = string.Empty;
            //get url
            TimeSpan ts = DateTime.Now - DateTime.Parse("1970-1-1");
            long tt = Convert.ToInt64(ts.TotalMilliseconds);
            string begintime = (DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + (DateTime.Now.Day - 1));
            string endTime = (DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + (DateTime.Now.Day - 1));
            string postUrl = "http://card.sdu.edu.cn/CardManage/CardInfo/TrjnList?"
                            +"beginTime="+begintime
                            +"&endTime="+endTime
                            +"&type=1"
                            +"&_="+tt.ToString();
            string todayUrl = "http://card.sdu.edu.cn/CardManage/CardInfo/TrjnList?"
                            + "&type=0"
                            + "&_=" + tt.ToString();

            //set request
            HttpRequestMessage _agoHttpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(postUrl));
            HttpRequestMessage _todayHttpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(todayUrl));

            //set cookie
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Name", _loadCookie.Name));
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Value", _loadCookie.Value));
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Domain", _loadCookie.Domain));
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Path", _loadCookie.Path));
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Expires", _loadCookie.Expires.ToString()));
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Secure", _loadCookie.Secure.ToString()));
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("HttpOnly", _loadCookie.HttpOnly.ToString()));

            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Name", _loginCookie.Name));
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Value", _loginCookie.Value));
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Domain", _loginCookie.Domain));
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Path", _loginCookie.Path));
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Expires", _loginCookie.Expires.ToString()));
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Secure", _loginCookie.Secure.ToString()));
            _agoHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("HttpOnly", _loginCookie.HttpOnly.ToString()));

            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Name", _loadCookie.Name));
            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Value", _loadCookie.Value));
            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Domain", _loadCookie.Domain));
            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Path", _loadCookie.Path));
            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Expires", _loadCookie.Expires.ToString()));
            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Secure", _loadCookie.Secure.ToString()));
            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("HttpOnly", _loadCookie.HttpOnly.ToString()));

            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Name", _loginCookie.Name));
            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Value", _loginCookie.Value));
            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Domain", _loginCookie.Domain));
            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Path", _loginCookie.Path));
            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Expires", _loginCookie.Expires.ToString()));
            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("Secure", _loginCookie.Secure.ToString()));
            _todayHttpRequest.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue("HttpOnly", _loginCookie.HttpOnly.ToString()));
            //Do request
            HttpResponseMessage _todayHttpResponse = await httpClient.SendRequestAsync(_todayHttpRequest);
            HttpResponseMessage _agoHttpResponse = await httpClient.SendRequestAsync(_agoHttpRequest);
            RecordSet.Clear();
            string todayResult = await _todayHttpResponse.Content.ReadAsStringAsync();
            anaylasHtml(todayResult);
            string agoResult = await _agoHttpResponse.Content.ReadAsStringAsync();
            anaylasHtml(agoResult);
            
            _isdowning.IsActive = false;
            return result;
        }

        

        private void RaisePropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region 简单处理
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

        private void anaylasHtml(string result)
        {
            string[] first_sentences = { "tbody" };
            string[] second_sentences = { "tr" };
            string[] third_sentences = { "td" };
            string[] RecordHtml = result.Split(first_sentences, StringSplitOptions.RemoveEmptyEntries)[1].Split(second_sentences, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 3; i < RecordHtml.Length; i += 2)
            {
                RecordSet.Add(HtmlToCardRecord(RecordHtml[i]));
            }
        }

        private CardRecord HtmlToCardRecord(string divHtml)
        {
            CardRecord _cardRecord = new CardRecord();

            string[] sentences = { "td" };
            string[] _record = divHtml.Split(sentences, StringSplitOptions.RemoveEmptyEntries);

            _cardRecord.c_time = _record[1].Substring(27, _record[1].Length - 51);
            _cardRecord.c_place = _record[3].Substring(27, _record[3].Length - 51);
            _cardRecord.c_type = _record[5].Substring(27, _record[5].Length - 51);
            _cardRecord.c_value = _record[7].Substring(45, _record[7].Length - 76);

            return _cardRecord;
        }
        #endregion

    }
}
