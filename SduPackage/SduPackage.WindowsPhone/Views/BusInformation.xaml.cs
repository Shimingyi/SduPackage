using SduPackage.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace SduPackage.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BusInformation : Page
    {
        BusViewModel _busViewModel;

        public BusInformation()
        {
            this.InitializeComponent();
            _busViewModel = new BusViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var searchBusNumString = e.Parameter as string;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            analysisNum(searchBusNumString);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        #region 事件
        public void analysisNum(string NumString)
        {
            string[] stringSeparators = new string[] { "to" };
            string[] PlaceNum = NumString.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            int startPlaceNum = Int32.Parse(PlaceNum[0]);
            int endPlaceNum = Int32.Parse(PlaceNum[1]);
            setStartPlace(startPlaceNum);
            setEndPlace(endPlaceNum);

            _busViewModel.SearchBus(startPlaceNum, endPlaceNum);
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
        #endregion

        #region 私有方法
        private void setStartPlace(int i){
            switch (i)
            {
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
