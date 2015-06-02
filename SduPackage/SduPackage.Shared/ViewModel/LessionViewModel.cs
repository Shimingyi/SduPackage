using Newtonsoft.Json.Linq;
using SduPackage.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SduPackage.ViewModel
{
    public class LessionViewModel : INotifyPropertyChanged
    {
        #region 字段
        private Windows.Storage.StorageFolder _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        private Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private int nowWeek;
        #endregion

        #region 属性
        public System.Collections.ObjectModel.ObservableCollection<MyClass> LessionGroups
        {
            get;
            private set;
        }
        #endregion

        #region 事件
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region 构造方法
        public LessionViewModel()
        {
            LoadViewModel();
            LessionGroups = new System.Collections.ObjectModel.ObservableCollection<MyClass>();
            this.RaisePropertyChanged("LessionGroups");
            LoadFile();
        }

        public LessionViewModel(Windows.UI.Xaml.Controls.ListView _listView)
        {
            LoadViewModel();
            LessionGroups = new System.Collections.ObjectModel.ObservableCollection<MyClass>();
            this.RaisePropertyChanged("LessionGroups");
            LoadFile(_listView);
        }
        #endregion

        #region 方法
        public void LoadFile(){
            OpenFile("TheLessionFile.txt");
        }

        public void LoadFile(Windows.UI.Xaml.Controls.ListView _listView)
        {
            OpenFile("TheLessionFile.txt",_listView);
        }
        #endregion

        #region 私有方法
        private void LoadViewModel()
        {
            if (_localSettings.Values.ContainsKey("nowWeek"))
            {
                nowWeek = Int32.Parse(_localSettings.Values["nowWeek"].ToString());
            }
            else
            {
                _localSettings.Values["nowWeek"] = 1;
            }
        }

        public async void OpenFile(string fileName)
        {
            Windows.Storage.StorageFile onlineNews = await _localFolder.GetFileAsync(fileName);
            string result = await Windows.Storage.FileIO.ReadTextAsync(onlineNews);
            FileTxtToGroup(result);
        }

        public async void OpenFile(string fileName, Windows.UI.Xaml.Controls.ListView _listView)
        {
            Windows.Storage.StorageFile onlineNews = await _localFolder.GetFileAsync(fileName);
            string result = await Windows.Storage.FileIO.ReadTextAsync(onlineNews);
            FileTxtToGroup(result,_listView);
        }

        private void FileTxtToGroup(string str)
        {
            DateTime dt = DateTime.Now;
            String data = dt.DayOfWeek.ToString();
            int count = 0;
            
            if (data == "Monday") { count = 1; }
            if (data == "Tuesday") { count = 2; }
            if (data == "Wednesday") { count = 3; }
            if (data == "Thursday") { count = 4; }
            if (data == "Friday") { count = 5; }
            if (data == "Saturday") { count = 6; }
            if (data == "Sunday") { count = 7; }

            str = str.Substring(1, str.Length - 2);
            JArray ja = JArray.Parse(str);
            JObject jo_temp = new JObject();
            for (int i = 0; i < ja.Count; i++)
            {
                jo_temp = ja[i] as JObject;
                if ((jo_temp["classDayOfWeek"].ToString() == count.ToString()) && isNowWeek(jo_temp["classDayOfSemester"].ToString()))
                {
                    LessionGroups.Add(JsonToLession(jo_temp));
                }
            }
        }

        private void FileTxtToGroup(string str, Windows.UI.Xaml.Controls.ListView _listView)
        {
            DateTime dt = DateTime.Now;
            String data = dt.DayOfWeek.ToString();
            int count = 0;

            if (data == "Monday") { count = 1; }
            if (data == "Tuesday") { count = 2; }
            if (data == "Wednesday") { count = 3; }
            if (data == "Thursday") { count = 4; }
            if (data == "Friday") { count = 5; }
            if (data == "Saturday") { count = 6; }
            if (data == "Sunday") { count = 7; }

            str = str.Substring(1, str.Length - 2);
            JArray ja = JArray.Parse(str);
            JObject jo_temp = new JObject();
            for (int i = 0; i < ja.Count; i++)
            {
                jo_temp = ja[i] as JObject;
                if ((jo_temp["classDayOfWeek"].ToString() == count.ToString()) && isNowWeek(jo_temp["classDayOfSemester"].ToString()))
                {
                    LessionGroups.Add(JsonToLession(jo_temp));
                }
            }
            if (LessionGroups.Count == 0)
            {
                LessionGroups.Add(new MyClass
                {
                    className = "虽然今天没有课，学习还是不能停止噢",
                    classPlace = "自习室",
                    classDayOfTime = 12345
                });
            }
            _listView.ItemsSource = LessionGroups;
        }

        private bool isNowWeek(string classDay)
        {
            bool res = false;
            if (classDay != "")
            {
                string[] shuzu = classDay.Split('-');
                int startWeek = Int32.Parse(shuzu[0]);
                int endWeek = Int32.Parse(shuzu[1]);
                if ((startWeek <= nowWeek) && (endWeek >= nowWeek))
                {
                    res = true;
                }
                else
                    res = false;
            }
            else
            {
                res = true;
            }
            return res;
        }

        private MyClass JsonToLession(JObject jo)
        {
            MyClass _myClass = new MyClass();
            _myClass.className = jo["className"].ToString();
            _myClass.classPlace = jo["classPlace"].ToString();
            _myClass.classDayOfWeek = Int32.Parse(jo["classDayOfWeek"].ToString());
            _myClass.classDayOfTime = Int32.Parse(jo["classDayOfTime"].ToString());
            _myClass.classDayOfSemester = jo["classDayOfSemester"].ToString();
            return _myClass;
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
