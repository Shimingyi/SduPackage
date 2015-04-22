using SduPackage.Model;
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
    public sealed partial class Index : Page
    {
        #region 字段
        Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        NewsViewModel _newsViewModel;
        DispatcherTimer dt = new DispatcherTimer();
        bool IsExit = false;
        News _news;
        #endregion


        public Index()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            InitWatch();
            _newsViewModel = new NewsViewModel(1);
            var groups = new System.Collections.ObjectModel.ObservableCollection<Group>();


            this.DataContext = _newsViewModel.NewsGroups;
;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Change_StatuBar("口袋山大", 0);
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        #region 页面逻辑事件
        private void refreshNews(object sender, RoutedEventArgs e)
        {
            _newsViewModel.refleshNews(1);
            this.DataContext = _newsViewModel.NewsGroups;
        }

        private void ViewNewsBody(object sender, TappedRoutedEventArgs e)
        {
            
            Grid grid = sender as Grid;
            _news = grid.DataContext as News;
            Frame.Navigate(typeof(Views.NewsBody), _news);
            //System.Diagnostics.Debug.WriteLine(_news.title);
        }

        private void ToBusSearchView(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.BusSearch));
        }

        private void HeaderTapped(object sender, TappedRoutedEventArgs e)
        {
            var btn = sender as SduPackage.Controls.HeaderButton;
            int index = int.Parse(btn.Tag.ToString());
            pivot.SelectedIndex = index;
            OnHeaderChanged(index);
        }

        private void pivot_PivotItemLoaded(Pivot sender, PivotItemEventArgs args)
        {
            if (pivot.SelectedIndex >= 0)
            {
                OnHeaderChanged(pivot.SelectedIndex);
            }
        }

        private void ToMyAccount(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.MyAccount));
        }

        private void ToMyLibrary(object sender, TappedRoutedEventArgs e)
        {
            if (_localSettings.Values.ContainsKey("CardUsername"))
            {
                Frame.Navigate(typeof(Library));
            }
            else
                NotifitionBar.ShowMessage("请先前往“我的账号”设置账号 >O<");
        }
        #endregion

        #region 自定义事件
        private void InitWatch()
        {
            dt.Interval = TimeSpan.FromSeconds(2);
            dt.Tick += (s, e) =>
            {
                if (IsExit)
                {
                    IsExit = false;
                    dt.Stop();
                }
            };
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (!IsExit)
            {
                NotifitionBar.ShowMessage("再按一次退出应用");
                IsExit = true;
                dt.Start();
                e.Handled = true;
            }
            else
            {
                dt.Stop();
                App.Current.Exit();
            }
        }

        private void LikeTheVideoTapped(object sender, TappedRoutedEventArgs e)
        {
            var btn = sender as SduPackage.Controls.HeaderButton;
            if (btn.IsChecked == true)
                btn.IsChecked = false;
            else
                btn.IsChecked = true;

        }

        
        #endregion

        #region 状态改变

        private async void Change_StatuBar(string str, int process)
        {
            Windows.UI.ViewManagement.StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = (Resources["ButtonPressedBackgroundThemeBrush"] as SolidColorBrush).Color;
            statusBar.ForegroundColor = Windows.UI.Colors.White;
            statusBar.BackgroundOpacity = 1;
            statusBar.ProgressIndicator.Text = str;
            statusBar.ProgressIndicator.ProgressValue = process;
            await statusBar.ProgressIndicator.ShowAsync();
        }


        private void OnHeaderChanged(int newIndex)
        {
            switch (newIndex)
            {
                case 0:
                    headerHome.IsChecked = true;
                    headerTodolist.IsChecked = false;
                    headerBox.IsChecked = false;
                    headerProfile.IsChecked = false;
                    AppBarChange(0);
                    break;
                case 1:
                    headerHome.IsChecked = false;
                    headerTodolist.IsChecked = true;
                    headerBox.IsChecked = false;
                    headerProfile.IsChecked = false;
                    AppBarChange(1);
                    break;
                case 2:
                    headerHome.IsChecked = false;
                    headerTodolist.IsChecked = false;
                    headerBox.IsChecked = true;
                    headerProfile.IsChecked = false;
                    AppBarChange(2);
                    break;
                case 3:
                    headerHome.IsChecked = false;
                    headerTodolist.IsChecked = false;
                    headerBox.IsChecked = false;
                    headerProfile.IsChecked = true;
                    AppBarChange(3);
                    break;
            }
        }

        private void AppBarChange(int index)
        {
            switch (index)
            {
                case 0:
                    IndexAppBar.CompositeMode = ElementCompositeMode.Inherit;
                    refreshAppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    myAccountAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
                case 1:
                    IndexAppBar.CompositeMode = ElementCompositeMode.MinBlend;
                    refreshAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    myAccountAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
                case 2:
                    IndexAppBar.CompositeMode = ElementCompositeMode.MinBlend;
                    refreshAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    myAccountAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
                case 3:
                    IndexAppBar.CompositeMode = ElementCompositeMode.Inherit;
                    refreshAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    myAccountAppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    break;
            }
        }
        #endregion

        

        

        

        
    }
}
