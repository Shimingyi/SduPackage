﻿using SduPackage.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    
    public sealed partial class Library : Page
    {

        BookViewModel _bookViewModel;
        Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        
        public Library()
        {
            this.InitializeComponent();
            this._bookViewModel = new BookViewModel();
            this.DataContext = _bookViewModel;

            LoadPage();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        #region 页面事件
        private void Search(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.LibrarySearch), key_search.Text);
        }

        private void ChooseBookAppBarClick(object sender, RoutedEventArgs e)
        {
            myNotifitionBar.ShowMessage("调试中，下个版本推出，敬请期待 0.0");
            /*
             bookListView.SelectionMode = ListViewSelectionMode.Multiple;
            search.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            choose.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            continuebar.Visibility = Windows.UI.Xaml.Visibility.Visible;
             */

        }

        private void ContinueBookAppBarClick(object sender, RoutedEventArgs e)
        {
            foreach (SduPackage.Model.BookInformation _book in bookListView.SelectedItems)
            {
                _bookViewModel.continueBook(_book, Windows.UI.ViewManagement.StatusBar.GetForCurrentView(), myNotifitionBar);                
            }
            _bookViewModel.downToFile(Windows.UI.ViewManagement.StatusBar.GetForCurrentView());
        }
        #endregion

        #region 方法
        private void LoadPage()
        {
            _bookViewModel.downToFile(Windows.UI.ViewManagement.StatusBar.GetForCurrentView());
            
        }

        

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if(bookListView.SelectionMode == ListViewSelectionMode.Multiple){
                bookListView.SelectionMode = ListViewSelectionMode.None;
                search.Visibility = Windows.UI.Xaml.Visibility.Visible;
                choose.Visibility = Windows.UI.Xaml.Visibility.Visible;
                continuebar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
            
        }
        #endregion

        #region 私有方法
        private void checkAccount()
        {
            
        }

        private async void Change_StatuBar(string str, double process)
        {
            Windows.UI.ViewManagement.StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = (Resources["ButtonPressedBackgroundThemeBrush"] as SolidColorBrush).Color;
            statusBar.ForegroundColor = Windows.UI.Colors.White;
            statusBar.BackgroundOpacity = 1;
            statusBar.ProgressIndicator.Text = str;
            statusBar.ProgressIndicator.ProgressValue = process;
            await statusBar.ProgressIndicator.ShowAsync();
        }
        #endregion

        

        
    }
}
