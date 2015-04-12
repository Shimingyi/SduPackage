using System;
using System.IO;
using System.Xml.Linq;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace SduPackage.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BusSetting : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        string BusDBSummary_now;
        string lastSummary;
        int lastSize;
        string lastUrl;
        

        public BusSetting()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            Change_StatuBar("口袋山大", 1);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            LoadPage();
        }

        #region 点击事件
        private void CheckSQLUpdate(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            getLastSummary(BusDBSummary_now, sender);
        }

        private void DownloadBudDB(object sender, RoutedEventArgs e)
        {
            NotificationFlyout.Hide();
            Change_StatuBar(lastSize+"kb的数据正在下载......",0.5);
            downLoadFile();
        }
        #endregion

        #region 事件
        public void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        public void LoadPage()
        {
            //检查数据库版本
            if (localSettings.Values.ContainsKey("BusDBSummary"))
            {
                BusDBSummary_now = localSettings.Values["BusDBSummary"].ToString();
                BudDBTextBlock.Text = BusDBSummary_now;
            }
        }

        public async void getLastSummary(string nowSummary,object sender)
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
                    NotificationText.Text = ("有一项"+lastSummary+"版本的数据库可以下载");
                    Windows.UI.Xaml.Controls.Primitives.FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
                }
                else
                {
                    NoNewBusDB.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
            catch (Exception e )
            {
                NotifitionBar.ShowMessage("网络连接情况不理想 >O<");
            }
        }

        public async void downLoadFile()
        {
            IBuffer buffer;
            HttpClient client = new HttpClient();
            try
            {
                buffer = await client.GetBufferAsync(new Uri(lastUrl));
                SaveFile("bus.db",buffer);
                localSettings.Values["BusDBSummary"] = lastSummary;
                System.Diagnostics.Debug.WriteLine("公交车数据库完成下载");
                BudDBTextBlock.Text = lastSummary;
                Change_StatuBar("下载完成",1);
            }
            catch (AggregateException e)
            {
                NotifitionBar.ShowMessage("下载失败 >O<");
            }
        } 
        #endregion

        #region 私有事件
        private async void SaveFile(string FileName, string result)
        {
            Windows.Storage.StorageFile tempFile = await localFolder.CreateFileAsync(FileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(tempFile, result);
        }

        private async void SaveFile(string FileName, IBuffer buffer)
        {
            Windows.Storage.StorageFile tempFile = await localFolder.CreateFileAsync(FileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteBufferAsync(tempFile,buffer);
        }
        #endregion

        #region 状态改变
        private async void Change_StatuBar(string str, double process)
        {
            Windows.UI.ViewManagement.StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = (Resources["ButtonPressedBackgroundThemeBrush"] as Windows.UI.Xaml.Media.SolidColorBrush).Color;
            statusBar.ForegroundColor = Windows.UI.Colors.White;
            statusBar.BackgroundOpacity = 1;
            statusBar.ProgressIndicator.Text = str;
            statusBar.ProgressIndicator.ProgressValue = process;
            await statusBar.ProgressIndicator.ShowAsync();
        }
        #endregion

        
    }
        
}
