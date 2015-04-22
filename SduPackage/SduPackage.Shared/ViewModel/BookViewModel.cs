using Newtonsoft.Json.Linq;
using SduPackage.Funcitons;
using SduPackage.Functions;
using SduPackage.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;
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
        public ObservableCollection<BusInformation> BookGroup
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
            RaisePropertyChanged("BookGroup");
            LoadBookGroup(index);
        }
    	#endregion

    	#region 方法
        public void LoadBookGroup(int index)
        {
            downToFile(index);
        }

        public void ContinueBook()
        {

        }

        public void ClearGroup()
        {

        }
    	#endregion

    	#region 私有方法
        private void downToFile(int index){
            var http = new DoPost();
            string username = _localSettings.Containers["LibraryUsername"].ToString();
            string password = _localSettings.Containers["LibraryPassword"].ToString();
            switch(index){
                case 1:
                    string url = ("http://202.194.14.195:8080/curriculumlib/lib");
                    string postContent = string.Format("requesttype=0&username={0}&password={1}", username, password);
                    http.StartPost(url, postContent, result =>
                    {
                        System.Diagnostics.Debug.WriteLine(result);
                        addToGroup(result);
                        SaveFile("MyLibraryFile.txt", result);
                    });
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }

        private void addToGroup(string result)
        {
            JArray ja = JArray.Parse(result);

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