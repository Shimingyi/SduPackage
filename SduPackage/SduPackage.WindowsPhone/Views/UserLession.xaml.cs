using Newtonsoft.Json.Linq;
using SduPackage.Model;
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
   
    public sealed partial class UserLession : Page
    {
        #region 字段
        LessionGroup _course = new LessionGroup();
        private Windows.Storage.StorageFolder _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        private Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private int nowWeek = 1;
        String timestamp = "";
        #endregion


        public UserLession()
        {
            this.InitializeComponent();
            LoadPage();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            checkWeek();
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _localSettings.Values["nowWeek"] = nowWeek;
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

        private void LastWeek(object sender, RoutedEventArgs e)
        {
            nowWeek--;
            checkWeek();
            this.LoadCourse(timestamp);
        }

        private void NextWeek(object sender, RoutedEventArgs e)
        {
            nowWeek++;
            checkWeek();
            this.LoadCourse(timestamp);
        }

        #region 方法
        public void LoadPage()
        {
            if (_localSettings.Values.ContainsKey("nowWeek"))
            {
                nowWeek = Int32.Parse(_localSettings.Values["nowWeek"].ToString());
            }
            else
            {
                _localSettings.Values["nowWeek"] = 1;
            }
            PageNum.Text = nowWeek.ToString();
            GetCourse();
        }
        #endregion

        #region 私有方法
        private async void GetCourse()
        {
            try
            {
                Windows.Storage.StorageFile sampleFile = await _localFolder.GetFileAsync("TheLessionFile.txt");
                timestamp = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
                this.LoadCourse(timestamp);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("读取课程信息文件错误");
            }
        }

        private void LoadCourse(string str)
        {
            _course.clear();
            str = str.Substring(1, str.Length - 2);
            JArray ja = JArray.Parse(str);
            JObject jo_temp = new JObject();
            for (int i = 0; i < ja.Count; i++)
            {
                jo_temp = ja[i] as JObject;
                int time = System.Int32.Parse(jo_temp["classDayOfTime"].ToString());
                if ((jo_temp["classDayOfWeek"].ToString() == "1") && isNowWeek(jo_temp["classDayOfSemester"].ToString()))
                {
                    this._course.Monday[time - 1] = JsonToClass(jo_temp);
                }
                if ((jo_temp["classDayOfWeek"].ToString() == "2") && isNowWeek(jo_temp["classDayOfSemester"].ToString()))
                {
                    this._course.Tuesday[time - 1] = JsonToClass(jo_temp);
                }
                if ((jo_temp["classDayOfWeek"].ToString() == "3") && isNowWeek(jo_temp["classDayOfSemester"].ToString()))
                {
                    this._course.Wednesday[time - 1] = JsonToClass(jo_temp);
                }
                if ((jo_temp["classDayOfWeek"].ToString() == "4") && isNowWeek(jo_temp["classDayOfSemester"].ToString()))
                {
                    this._course.Thursday[time - 1] = JsonToClass(jo_temp);
                }
                if ((jo_temp["classDayOfWeek"].ToString() == "5") && isNowWeek(jo_temp["classDayOfSemester"].ToString()))
                {
                    this._course.Friday[time - 1] = JsonToClass(jo_temp);
                }
                if ((jo_temp["classDayOfWeek"].ToString() == "6") && isNowWeek(jo_temp["classDayOfSemester"].ToString()))
                {
                    this._course.Saturday[time - 1] = JsonToClass(jo_temp);
                }
                if ((jo_temp["classDayOfWeek"].ToString() == "7") && isNowWeek(jo_temp["classDayOfSemester"].ToString()))
                {
                    this._course.Sunday[time - 1] = JsonToClass(jo_temp);
                }
            }
            update();
        }

        private bool isNowWeek(string classDay)
        {
            bool res = false;
            if(classDay!=""){
                string[] shuzu = classDay.Split('-');
                int startWeek = Int32.Parse(shuzu[0]);
                int endWeek = Int32.Parse(shuzu[1]);
                if ((startWeek <= nowWeek) && (endWeek >= nowWeek))
                {
                    res = true;
                }
                else
                    res = false;
            }
            else
            {
                res = true;
            }
            return res;
        }

        private MyClass JsonToClass(JObject jo)
        {
            MyClass _myClass = new MyClass();
            _myClass.className = jo["className"].ToString();
            _myClass.classPlace = jo["classPlace"].ToString();
            _myClass.classDayOfWeek = Int32.Parse(jo["classDayOfWeek"].ToString());
            _myClass.classDayOfTime = Int32.Parse(jo["classDayOfTime"].ToString());
            _myClass.classDayOfSemester = jo["classDayOfSemester"].ToString();
            return _myClass;
        }
        #endregion

        #region 状态提醒
        private void checkWeek()
        {
            PageNum.Text = nowWeek.ToString();
            lastAppbar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            nextAppbar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if((nowWeek <=1) ){
                lastAppbar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            if((nowWeek >= 20)){
                nextAppbar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }


        private void update()
        {
            Monday1.Content = _course.Monday[0].className + " " + _course.Monday[0].classPlace;
            Monday2.Content = _course.Monday[1].className + " " + _course.Monday[1].classPlace;
            Monday3.Content = _course.Monday[2].className + " " + _course.Monday[2].classPlace;
            Monday4.Content = _course.Monday[3].className + " " + _course.Monday[3].classPlace;
            Monday5.Content = _course.Monday[4].className + " " + _course.Monday[4].classPlace;
            Monday6.Content = _course.Monday[5].className + " " + _course.Monday[5].classPlace;

            Tuesday1.Content = _course.Tuesday[0].className + " " + _course.Tuesday[0].classPlace;
            Tuesday2.Content = _course.Tuesday[1].className + " " + _course.Tuesday[1].classPlace;
            Tuesday3.Content = _course.Tuesday[2].className + " " + _course.Tuesday[2].classPlace;
            Tuesday4.Content = _course.Tuesday[3].className + " " + _course.Tuesday[3].classPlace;
            Tuesday5.Content = _course.Tuesday[4].className + " " + _course.Tuesday[4].classPlace;
            Tuesday6.Content = _course.Tuesday[5].className + " " + _course.Tuesday[5].classPlace;

            Wednesday1.Content = _course.Wednesday[0].className + " " + _course.Wednesday[0].classPlace;
            Wednesday2.Content = _course.Wednesday[1].className + " " + _course.Wednesday[1].classPlace;
            Wednesday3.Content = _course.Wednesday[2].className + " " + _course.Wednesday[2].classPlace;
            Wednesday4.Content = _course.Wednesday[3].className + " " + _course.Wednesday[3].classPlace;
            Wednesday5.Content = _course.Wednesday[4].className + " " + _course.Wednesday[4].classPlace;
            Wednesday6.Content = _course.Wednesday[5].className + " " + _course.Wednesday[5].classPlace;

            Thursday1.Content = _course.Thursday[0].className + " " + _course.Thursday[0].classPlace;
            Thursday2.Content = _course.Thursday[1].className + " " + _course.Thursday[1].classPlace;
            Thursday3.Content = _course.Thursday[2].className + " " + _course.Thursday[2].classPlace;
            Thursday4.Content = _course.Thursday[3].className + " " + _course.Thursday[3].classPlace;
            Thursday5.Content = _course.Thursday[4].className + " " + _course.Thursday[4].classPlace;
            Thursday6.Content = _course.Thursday[5].className + " " + _course.Thursday[5].classPlace;

            Friday1.Content = _course.Friday[0].className + " " + _course.Friday[0].classPlace;
            Friday2.Content = _course.Friday[1].className + " " + _course.Friday[1].classPlace;
            Friday3.Content = _course.Friday[2].className + " " + _course.Friday[2].classPlace;
            Friday4.Content = _course.Friday[3].className + " " + _course.Friday[3].classPlace;
            Friday5.Content = _course.Friday[4].className + " " + _course.Friday[4].classPlace;
            Friday6.Content = _course.Friday[5].className + " " + _course.Friday[5].classPlace;

            Saturday1.Content = _course.Saturday[0].className + " " + _course.Saturday[0].classPlace;
            Saturday2.Content = _course.Saturday[1].className + " " + _course.Saturday[1].classPlace;
            Saturday3.Content = _course.Saturday[2].className + " " + _course.Saturday[2].classPlace;
            Saturday4.Content = _course.Saturday[3].className + " " + _course.Saturday[3].classPlace;
            Saturday5.Content = _course.Saturday[4].className + " " + _course.Saturday[4].classPlace;
            Saturday6.Content = _course.Saturday[5].className + " " + _course.Saturday[5].classPlace;

            Sunday1.Content = _course.Sunday[0].className + " " + _course.Sunday[0].classPlace;
            Sunday2.Content = _course.Sunday[1].className + " " + _course.Sunday[1].classPlace;
            Sunday3.Content = _course.Sunday[2].className + " " + _course.Sunday[2].classPlace;
            Sunday4.Content = _course.Sunday[3].className + " " + _course.Sunday[3].classPlace;
            Sunday5.Content = _course.Sunday[4].className + " " + _course.Sunday[4].classPlace;
            Sunday6.Content = _course.Sunday[5].className + " " + _course.Sunday[5].classPlace;
        }
        #endregion

        
    }
}
