using Newtonsoft.Json.Linq;
using SduPackage.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SduPackage.ViewModel
{
    public class GradeViewModel : INotifyPropertyChanged
    {
        #region 字段
        private Windows.Storage.StorageFolder _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        #endregion

        #region 属性
        public System.Collections.ObjectModel.ObservableCollection<Group> GradeGroups
        {
            get;
            private set;
        }
        #endregion

        #region 事件
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region 构造方法
        public GradeViewModel()
        {
            GradeGroups = new System.Collections.ObjectModel.ObservableCollection<Group>();
            this.RaisePropertyChanged("GradeGroups");
            LoadFile();
        }
        #endregion

        #region 方法
        public void LoadFile()
        {
            OpenFile("TheGradeFile.txt");
        }
        #endregion

        #region 私有方法
        private async void OpenFile(string filename)
        {
            Windows.Storage.StorageFile onlineNews = await _localFolder.GetFileAsync(filename);
            string result = await Windows.Storage.FileIO.ReadTextAsync(onlineNews);
            System.Diagnostics.Debug.WriteLine("myGrade:"+result);
            FileTxtToGroup(result);
        }

        private void FileTxtToGroup(string result)
        {
            string[] stringSeparators = new string[] { "学生在线课程格子" };
            string[] temp = result.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 2; i < 12; i++)
            {
                result = temp[i];
                result = result.Substring(1, result.Length - 2);
                if (!((result=="[]") || (result=="null")))
                {
                    JArray ja = JArray.Parse(result);
                    JObject jo = ja[0] as JObject;
                    Group tempGroup = new Group
                    {
                        Name = (jo["classYear"] + "学年" + jo["semester"] + "学期")
                    };
                    for (int j = 0; j < ja.Count; j++)
                    {
                        JObject tempJo = ja[j] as JObject;
                        tempGroup.GradeItems.Add(JsonToGrade(tempJo));
                    }
                    GradeGroups.Add(tempGroup);
                }
            }
        }

        private MyPoint JsonToGrade(JObject jo)
        {
            MyPoint _mypoint = new MyPoint();
            _mypoint.className = jo["className"].ToString();
            _mypoint.classYear =Int32.Parse(jo["classYear"].ToString());
            _mypoint.classSemester = Int32.Parse(jo["semester"].ToString());
            _mypoint.classCredit = jo["classCredit"].ToString();
            _mypoint.classAttitude = jo["classAttitude"].ToString();
            _mypoint.classGrade = jo["classGrade"].ToString();
            _mypoint.examTime = jo["examTime"].ToString();
            return _mypoint;
        }

        private void RaisePropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
