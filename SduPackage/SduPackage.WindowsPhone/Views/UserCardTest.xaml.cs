
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace SduPackage.Views
{


    public sealed partial class UserCardTest : Page
    {

        Windows.Web.Http.Filters.HttpBaseProtocolFilter filter;
        HttpClient httpclient;

        public UserCardTest()
        {
            this.InitializeComponent();
            DownloadImage();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
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

        void DownloadImage()
        {
            TimeSpan ts = DateTime.Now - DateTime.Parse("1970-1-1");
            long tt = Convert.ToInt64(ts.TotalMilliseconds);
            Uri imageUri = new Uri(("http://card.sdu.edu.cn/Account/GetCheckCodeImg/Flag="+tt.ToString()));
            Windows.UI.Xaml.Media.ImageSource _imgSource = new BitmapImage(imageUri);
            ShowImage.Source = _imgSource;
        }

        private async void LoginButton(object sender, RoutedEventArgs e)
        {
            filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            httpclient = new HttpClient(filter);
            try
            {
                string posturl = "http://card.sdu.edu.cn/Account/MiniCheckIn";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(posturl));
                HttpFormUrlEncodedContent postDate = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("signtype","SynCard"),
                        new KeyValuePair<string,string>("username","232123"),
                        new KeyValuePair<string,string>("password","641806"),
                        new KeyValuePair<string,string>("checkcode",ImageText.Text),
                        new KeyValuePair<string,string>("isUsedKeyPad","false")
                    }
                );
                request.Content = postDate;
                HttpResponseMessage response = await httpclient.SendRequestAsync(request);
                string responseStr = await response.Content.ReadAsStringAsync();
                if (responseStr.Substring(0, 7) == "success")
                {
                    httpclient = new HttpClient(filter);
                    responseStr = await httpclient.GetStringAsync(new Uri("http://card.sdu.edu.cn/CardManage/CardInfo/BasicInfo"));
                    NotificationText.Text = responseStr;
                }
                

            }
            catch (Exception args)
            {
                System.Diagnostics.Debug.WriteLine(args.ToString());
            }

        }
    }
}
