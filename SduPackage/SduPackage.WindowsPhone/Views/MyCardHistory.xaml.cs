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
using Windows.Web.Http;
using SduPackage.Model;
using SduPackage.ViewModel;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace SduPackage.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MyCardHistory : Page
    {
        CardViewModel _cardViewmodel;

        public MyCardHistory()
        {
            this.InitializeComponent();
            _cardViewmodel = new CardViewModel();
            this.DataContext = _cardViewmodel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HttpCookie[] _myCookies = e.Parameter as HttpCookie[];
            if(_myCookies.Length == 2){
                LoadPage(_myCookies);
            }
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }            
        }

        public void LoadPage(HttpCookie[] _myCookies)
        {
            _cardViewmodel.LoadRecord(_myCookies[0], _myCookies[1],isDowning);
        }
    }
}
