using Newtonsoft.Json.Linq;
using SduPackage.Funcitons;
using SduPackage.Functions;
using SduPackage.Functions.MyException;
using SduPackage.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace SduPackage.ViewModel{
	public class BookViewModel: INotifyPropertyChanged
    {
    	#region 字段
    	Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
    	#endregion

    	#region 属性
        public ObservableCollection<BookInformation> BookGroup
        {
            get;
            private set;
        }
    	#endregion
    	
    	#region 事件
    	public event PropertyChangedEventHandler PropertyChanged;
    	#endregion

    	#region 构造方法
        public BookViewModel(int index)
        {
            this.BookGroup = new ObservableCollection<BookInformation>();
            this.RaisePropertyChanged("BookGroup");
            LoadViewModel(index);
        }
    	#endregion

    	#region 方法
        public void LoadViewModel(int index)
        {
            downToFile(index);
        }

        public void SearchBook(string keywords)
        {
            SearchByPost(keywords);
        }

        public void ContinueBook()
        {

        }

        public void ClearGroup()
        {

        }
    	#endregion

    	#region 私有方法
        public async void downToFile(int index)
        {
            switch (index)
            {
                case 1:
                    try
                    {
                        string username = _localSettings.Values["CardUsername"].ToString();
                        string password = _localSettings.Values["CardPassword"].ToString();
                        HttpClient httpclient = new HttpClient();
                        string posturl = "http://202.194.14.195:8080/curriculumlib/lib";
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(posturl));
                        HttpFormUrlEncodedContent postDate = new HttpFormUrlEncodedContent(
                            new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("requesttype","0"),
                        new KeyValuePair<string,string>("username",username),
                        new KeyValuePair<string,string>("password",password),
                    }
                        );
                        request.Content = postDate;
                        HttpResponseMessage response = await httpclient.SendRequestAsync(request);
                        string result = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine(result);
                        if (result.IndexOf("错误") > 0)
                        {
                            throw new ErrorAccountException();
                        }
                        else
                        {
                            addToGroup(result,1);
                            SaveFile("MyLibraryFile.txt", result);
                        }
                    }
                    catch (ErrorAccountException args)
                    {

                    }
                    break;
                case 2:
                    
                    break;
                case 3:
                    break;
            }
            
        }

        private async void LoadFile(int index)
        {
            switch (index)
            {
                case 1:
                    try
                    {
                        StorageFile onlineNews = await _localFolder.GetFileAsync("MyLibraryFile.txt");
                        string timestamp = await FileIO.ReadTextAsync(onlineNews);
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        System.Diagnostics.Debug.WriteLine("First USE");
                    }
                    break;

            }
        }

        private async void SearchByPost(string keyword)
        {
            try
            {
                HttpClient httpclient = new HttpClient();
                string posturl = "http://202.194.14.195:8080/curriculumlib/lib";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(posturl));
                HttpFormUrlEncodedContent postDate = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("requesttype","2"),
                        new KeyValuePair<string,string>("keyword",keyword),
                        new KeyValuePair<string,string>("searchtype","5"),
                    }
                );
                request.Content = postDate;
                HttpResponseMessage response = await httpclient.SendRequestAsync(request);
                string result = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(result);
                addToGroup(result,2);
            }
            catch (ErrorAccountException args)
            {

            }
        }

        private void addToGroup(string result,int index)
        {
            try
            {
                JArray ja = JArray.Parse(result);
                switch (index)
                {
                    case 1:
                        for (int i = 0; i < ja.Count; i++)
                        {
                            JObject jo = ja[i] as JObject;
                            BookGroup.Add(JsonToBook(jo));
                        }
                        break;
                    case 2:
                        for (int i = 0; i < ja.Count-1; i++)
                        {
                            JObject jo = ja[i] as JObject;
                            BookGroup.Add(JsonToSearch(jo));
                        }
                        break;
                }                
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine("JSON格式错误");
            }
        }

        private BookInformation JsonToBook(JObject jo)
        {
            BookInformation _bookInformation = new BookInformation();

            _bookInformation.b_id = jo["id"].ToString();
            string[] shuzu = jo["title"].ToString().Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            _bookInformation.b_title = shuzu[0];
            _bookInformation.b_autor = shuzu[1];
            _bookInformation.b_data = jo["date"].ToString();
            return _bookInformation;
        }

        private BookInformation JsonToSearch(JObject jo)
        {
            BookInformation _bookInformation = new BookInformation();
            _bookInformation.b_total = jo["total"].ToString();
            _bookInformation.b_title = jo["title"].ToString();
            _bookInformation.b_detailurl = jo["detailurl"].ToString();
            _bookInformation.b_booknumber = jo["booknumber"].ToString();
            _bookInformation.b_canborrow = jo["canborrow"].ToString();

            return _bookInformation;
        }

        private async void SaveFile(string FileName, string result)
        {
            Windows.Storage.StorageFile tempFile = await _localFolder.CreateFileAsync(FileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(tempFile, result);
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