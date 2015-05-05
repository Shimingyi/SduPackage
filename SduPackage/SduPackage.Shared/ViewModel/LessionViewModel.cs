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
            LessionGroups = new System.Collections.ObjectModel.ObservableCollection<MyClass>();
            this.RaisePropertyChanged("LessionGroups");
            LoadFile();
        }
        #endregion

        #region 方法
        public void LoadFile(){
            OpenFile("TheLessionFile.txt");
        }
        #endregion

        #region 私有方法
        private async void OpenFile(string fileName)
        {
            Windows.Storage.StorageFile onlineNews = await _localFolder.GetFileAsync(fileName);
            string result = await Windows.Storage.FileIO.ReadTextAsync(onlineNews);
            FileTxtToGroup(result);
        }

        private void FileTxtToGroup(string result)
        {
            //For Test
            for (var i = 0; i < 49; i++)
            {
                LessionGroups.Add(new MyClass
                {
                    className = "Name " + i.ToString(),
                    classPlace = "Place " + i.ToString(),
                });
            }
        }

        private MyClass JsonToLession(JObject jo)
        {
            MyClass _myClass = new MyClass();

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
