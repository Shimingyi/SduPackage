using System;
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

        public BusSetting()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            LoadPage();
        }

        #region 点击事件
        private void CheckSQLUpdate(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
        }
        #endregion

        #region 事件
        public async void getLastSummary(string nowSummary)
        {
            string result = null;
            HttpClient client = new HttpClient();
            try
            {
                result = await client.GetStringAsync(new Uri("http://202.194.14.195:8080/schoolbus_update/update_db.xml"));
                
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("读取校车地址时错误");
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

        public void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        
        #endregion
    }
        
}
