using SduPackage.Model;
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
            _bookViewModel.SearchBook(keyword);
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void ViewBookInformation(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            BookInformation _book = grid.DataContext as BookInformation;
            Frame.Navigate(typeof(Views.LibraryInformation), _book);
        }

        private void Search(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _bookViewModel.SearchBook(key_search.Text);
            this.DataContext = _bookViewModel;
        }

        private void LastPage(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _bookViewModel.LastBook();
            this.DataContext = _bookViewModel;
        }

        private void NextPage(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _bookViewModel.NextBook();
            this.DataContext = _bookViewModel;
        }

        



    }
}
