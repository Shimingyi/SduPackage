using Newtonsoft.Json.Linq;
using SduPackage.Funcitons;
using SduPackage.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SduPackage.ViewModel
{
    public class NewsViewModel : INotifyPropertyChanged
    {

        #region 字段
        private StorageFolder _localFolder;
        #endregion

        #region 属性
        public ObservableCollection<Group> NewsGroups
        {
            get;
            private set;
        }
        #endregion

        #region 事件
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region 构造方法
        public NewsViewModel(int index)
        {
            this._localFolder = ApplicationData.Current.LocalFolder;
            this.NewsGroups = new ObservableCollection<Group>();
            this.RaisePropertyChanged("NewsGroups");
            LoadNews(index);
        }
        #endregion

        #region 方法
        public void LoadNews(int index)
        {
            LoadFile(index);
        }

        public void refleshNews(int index)
        {
            NewsGroups.Clear();
            downToFile(index);
            LoadFile(index);
        }
        #endregion

        #region 私有方法
        private async void LoadFile(int index)
        {
            switch (index)
            {
                case 1:
                    try
                    {
                        StorageFile onlineNews = await _localFolder.GetFileAsync("TheNewsFromSduOnline.txt");
                        string timestamp = await FileIO.ReadTextAsync(onlineNews);
                        StorageFile otherNews = await _localFolder.GetFileAsync("TheNewsFromOthers.txt");
                        string timestamp1 = await FileIO.ReadTextAsync(otherNews);
                        downNewsList(1, timestamp);
                        downNewsList(2, timestamp1);
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        System.Diagnostics.Debug.WriteLine("First USE");
                    }
                    
                    break;
                     
            }
            
        }

        private void downNewsList(int index, string result)
        {
            switch (index)
            {
                case 1:
                    var OneGroup = new Group
                    {
                        Name = "学生在线"
                    };
                    //System.Diagnostics.Debug.WriteLine("One:"+result);
                    JArray ja = JArray.Parse(result);
                    for (int i = 0; i < 10; i++)
                    {
                        JObject jo = ja[i] as JObject;
                        OneGroup.NewsItems.Add(jsonToNews(jo));
                    }
                    NewsGroups.Add(OneGroup);
                    break;
                case 2:
                    var TwoGroup = new Group
                    {
                        Name = "其他通知"
                    };
                    ja = JArray.Parse(result);
                    //System.Diagnostics.Debug.WriteLine("Two"+result);
                    for (int i = 0; i < 10; i++)
                    {
                        JObject jo = ja[i] as JObject;
                        TwoGroup.NewsItems.Add(jsonToNews(jo));
                    }
                    NewsGroups.Add(TwoGroup);
                    break;
            }
        }

        private void downToFile(int index)
        {
            switch (index)
            {
                case 1:
                    var http = new DoPost();
                    //学线通知
                    http.StartPost("http://www.online.sdu.edu.cn/News2s/servlet/NewestListServlet", "", result =>
                    {
                        result = result.Substring(2, result.Length - 2);
                        //downNewsList(1, result);
                        SaveFile("TheNewsFromSduOnline.txt", result);
                    });
                    http.StartPost("http://www.online.sdu.edu.cn/News2s/servlet/OtherListServlet", "id=11&page=1", result =>
                    {
                        result = result.Substring(2,result.Length-2);
                        //downNewsList(2, result);
                        SaveFile("TheNewsFromOthers.txt", result);
                    });
                    break;
                     
                case 2:

                    break;
            }
        }

        private async void SaveFile(string FileName, string result)
        {
            StorageFile tempFile = await _localFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(tempFile, result);
        }

        private News jsonToNews(JObject jo)
        {
            News _news = new News();
            _news.blockId = Int32.Parse(jo["blockId"].ToString());
            _news.pid = Int32.Parse(jo["pid"].ToString());
            _news.priorty = Int32.Parse(jo["priorty"].ToString());
            _news.title = jo["title"].ToString();
           // _news.content = jo["content"].ToString();
           // _news.addTime = jo["addTime"].ToString();
            _news.editTime = jo["editTime"].ToString();
            _news.subTitle = jo["subTitle"].ToString();
           // _news.editor = jo["editor"].ToString();
            _news.reporter = jo["reporter"].ToString();
            _news.source = jo["source"].ToString();
           // _news.keyWord = jo["keyWord"].ToString();

            return _news;
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
