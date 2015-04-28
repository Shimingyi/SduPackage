using SduPackage.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace SduPackage.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LibrarySearch : Page
    {
        BookViewModel _bookViewModel;


        public LibrarySearch()
        {
            this.InitializeComponent();
            _bookViewModel = new BookViewModel();
            this.DataContext = _bookViewModel;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            string keyword = e.Parameter as string;
            System.Diagnostics.Debug.WriteLine("Input:" + keyword);
            _bookViewModel = new BookViewModel();
            _bookViewModel.SearchBook(keyword);
            this.DataContext = _bookViewModel;

        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void Search(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _bookViewModel.SearchBook(key_search.Text);
        }



    }
}
