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

namespace SduPackage.ViewModel
{
    public class BusViewModel : INotifyPropertyChanged
    {
        #region 字段
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        string nowSummary;
        string lastSummary;
        int lastSize;
        string lastUrl;

        BusSqlite _busSqlite;
        #endregion

        #region 属性
        public ObservableCollection<BusInformation> WorkDayBuses
        {
            get;
            private set;
        }
        public ObservableCollection<BusInformation> WeekendBuses
        {
            get;
            private set;
        }

        #endregion

        #region 事件
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region 构造方法
        public BusViewModel()
        {
            _busSqlite = new BusSqlite();
            this.WorkDayBuses = new ObservableCollection<BusInformation>();
            this.WeekendBuses = new ObservableCollection<BusInformation>();
            this.RaisePropertyChanged("WorkDayBuses");
            this.RaisePropertyChanged("WeekendBuses");
        }
        #endregion
        
        #region 方法
        public void SearchBus(int startNum, int endNum)
        {
            List<BusInformation> busInformations = _busSqlite.SearchBus(startNum, endNum);
            for (int i = 0; i < busInformations.Count;i++ )
            {
                if (busInformations[i].bus_type == "1")
                {
                    WorkDayBuses.Add(busInformations[i]);
                }
                else
                {
                    WeekendBuses.Add(busInformations[i]);
                }
            }
        }

        public void UpdataSummary(){
            CheckDownloadFile();
        }
        #endregion

        #region 私有方法
        private async void CheckDownloadFile()
        {
            string result = null;
            HttpClient client = new HttpClient();
            try
            {
                result = await client.GetStringAsync(new Uri("http://202.194.14.195:8080/schoolbus_update/update_db.xml"));
                //System.Diagnostics.Debug.WriteLine("Get BusDBInformation:"+result);
                XDocument summaryXml = XDocument.Parse(result);
                XElement root = summaryXml.Element("db");
                XElement lastDBSummary = root.Element("version");
                XElement lastDBSize = root.Element("size");
                XElement lastDBUrl = root.Element("url");

                lastSummary = lastDBSummary.Value.ToString();
                lastSize = (Int32.Parse(lastDBSize.Value.ToString())) / 1024;
                lastUrl = lastDBUrl.Value.ToString();

                if (lastSummary != nowSummary)
                {
                    downLoadFile();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("数据库已是最新");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("获取数据库信息时出错");
            }
        }

        private async void downLoadFile()
        {
            IBuffer buffer;
            HttpClient client = new HttpClient();
            try
            {
                buffer = await client.GetBufferAsync(new Uri(lastUrl));
                SaveFile("bus.db", buffer);
                localSettings.Values["BusDBSummary"] = lastSummary;
                System.Diagnostics.Debug.WriteLine("公交车数据库完成下载");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("下载数据库时出错");
            }
        }

        private async void SaveFile(string FileName, string result)
        {
            Windows.Storage.StorageFile tempFile = await localFolder.CreateFileAsync(FileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(tempFile, result);
        }

        private async void SaveFile(string FileName, IBuffer buffer)
        {
            Windows.Storage.StorageFile tempFile = await localFolder.CreateFileAsync(FileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteBufferAsync(tempFile, buffer);
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
