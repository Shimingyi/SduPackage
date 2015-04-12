using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace SduPackage.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BusSearch : Page
    {
        int count = 0;
        int startPlaceNum, endPlaceNum;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public BusSearch()
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
            ResetPage();
        }
        #region 页面点击事件
        private void PlaceButtonTap(object sender, TappedRoutedEventArgs e)
        {
            if (CheckBusDB())
            {
                count++;
                var choosePlaceBorder = sender as Border;
                int choosePlaceNum = Int32.Parse((string)choosePlaceBorder.Tag);
                if ((count % 2) == 1)
                {
                    setStartPlace(choosePlaceNum);
                    startPlaceNum = choosePlaceNum;
                }
                else
                {
                    endPlaceNum = choosePlaceNum;
                    if (endPlaceNum != startPlaceNum)
                        Frame.Navigate(typeof(Views.BusInformation), startPlaceNum + "to" + endPlaceNum);
                    else
                        NotifitionBar.ShowMessage("原地绕圈不需要校车 >O<");
                }
            }
        }

        private void ToBusSettingPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.BusSetting));
        }
        #endregion

        #region 事件
        

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
        #endregion

        #region 私有事件
        private bool CheckBusDB()
        {
            if (localSettings.Values.ContainsKey("BusDBSummary"))
            {
                return true;
            }
            else
            {
                NotifitionBar.ShowMessage("还未下载数据库 >O<");
                return false;
            }
                
        }
        #endregion

        #region 状态改变事件
        private void ResetPage()
        {
            setStartPlace(0);
            setEndPlace(0);
            startPlaceNum = 0;
            endPlaceNum = 0;
        }

        private void setStartPlace(int i)
        {
            switch(i){
                case 0:
                    StartPlaceTextBlock.Text = "出发地";
                    break;
                case 1:
                    StartPlaceTextBlock.Text = "中心校区";
                    break;
                case 2:
                    StartPlaceTextBlock.Text = "洪家楼校区";
                    break;
                case 3:
                    StartPlaceTextBlock.Text = "千佛山校区";
                    break;
                case 4:
                    StartPlaceTextBlock.Text = "兴隆山校区";
                    break;
                case 5:
                    StartPlaceTextBlock.Text = "软件园校区";
                    break;
                case 6:
                    StartPlaceTextBlock.Text = "趵突泉校区";
                    break;
            }
               
        }

        private void setEndPlace(int i)
        {
            switch (i)
            {
                case 0:
                    EndPlaceTextBlock.Text = "目的地";
                    break;
                case 1:
                    EndPlaceTextBlock.Text = "中心校区";
                    break;
                case 2:
                    EndPlaceTextBlock.Text = "洪家楼校区";
                    break;
                case 3:
                    EndPlaceTextBlock.Text = "千佛山校区";
                    break;
                case 4:
                    EndPlaceTextBlock.Text = "兴隆山校区";
                    break;
                case 5:
                    EndPlaceTextBlock.Text = "软件园校区";
                    break;
                case 6:
                    EndPlaceTextBlock.Text = "趵突泉校区";
                    break;
            }

        }

        #endregion

        

    }
}
