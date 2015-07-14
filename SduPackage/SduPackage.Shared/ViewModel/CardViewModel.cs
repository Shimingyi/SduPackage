using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SduPackage.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.UI.Xaml.Controls;

namespace SduPackage.ViewModel
{
    public class CardViewModel : INotifyPropertyChanged
    {
        #region 字段
        
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
            this.RecordSet = new ObservableCollection<CardRecord>();
            this.RaisePropertyChanged("RecordSet");
        }
        #endregion

        #region 方法

        public async void LoadRecord(HttpCookie _loadCookie, HttpCookie _loginCookie , ProgressRing _isdowning)
        {
            await GetRecordHtml(_loadCookie, _loginCookie,_isdowning);
        }

        /*
        public async Task<string> LoadRecord(HttpCookie _loadCookie, HttpCookie _loginCookie)
        {
            string warn = string.Empty;
            string result = this.GetRecordHtml(_loadCookie,_loginCookie).Result;
            anaylasHtml(result);
            return warn;
        }
        */
        #endregion

        #region 私有方法
        public async Task<string> GetRecordHtml(HttpCookie _loadCookie, HttpCookie _loginCookie, ProgressRing _isdowning)
        {
            string result = string.Empty;
            HttpClient httpClient = new HttpClient();

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
            string todayResult = await _todayHttpResponse.Content.ReadAsStringAsync();
            anaylasHtml(todayResult);
            string agoResult = await _agoHttpResponse.Content.ReadAsStringAsync();
            anaylasHtml(agoResult);
            
            _isdowning.IsActive = false;
            return result;
        }

        private void anaylasHtml(string result)
        {
            string[] first_sentences = { "tbody" };
            string[] second_sentences = { "tr" };
            string[] third_sentences = { "td" };
            string[] RecordHtml = result.Split(first_sentences, StringSplitOptions.RemoveEmptyEntries)[1].Split(second_sentences, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 3; i < RecordHtml.Length;i+=2)
            {
                RecordSet.Add(HtmlToCardRecord(RecordHtml[i]));
            }
            
        }

        private CardRecord HtmlToCardRecord(string divHtml){
            CardRecord _cardRecord = new CardRecord();

            string[] sentences = { "td" };
            string[] _record = divHtml.Split(sentences,StringSplitOptions.RemoveEmptyEntries);
            
            _cardRecord.c_time = _record[1].Substring(27, _record[1].Length - 51);
            _cardRecord.c_place = _record[3].Substring(27, _record[3].Length - 51);
            _cardRecord.c_type = _record[5].Substring(27,_record[5].Length - 51);
            _cardRecord.c_value = _record[7].Substring(45, _record[7].Length - 76);

            return _cardRecord;
        }

        private void RaisePropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
