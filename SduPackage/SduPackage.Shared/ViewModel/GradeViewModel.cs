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
        private Windows.Storage.StorageFolder _localFolder;
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

        }
        #endregion

        #region 方法
        public void LoadFile()
        {
            OpenFile("TheInformationFile.txt");
        }
        #endregion

        #region 私有方法
        private async void OpenFile(string filename)
        {
            Windows.Storage.StorageFile onlineNews = await _localFolder.GetFileAsync(filename);
            string result = await Windows.Storage.FileIO.ReadTextAsync(onlineNews);
            FileTxtToGroup(result);
        }

        private void FileTxtToGroup(string result)
        {
            JArray ja = JArray.Parse(result);
            Group tempGroup = new Group();
            int AgoYear = 0, AgoSemester = 0;
            int nowYear = 0, nowSemester = 0;
            int count = 0;
            for (int i = 0; i < ja.Count; i++)
            {
                JObject jo = ja[i] as JObject;
                AgoYear =Int32.Parse(jo["classYear"].ToString());
                AgoSemester = Int32.Parse(jo["semester"].ToString());
                if ((nowYear != AgoYear) || (nowSemester != AgoSemester))
                {
                    if (tempGroup.GradeItems.Count != 0)
                    {
                        GradeGroups.Add(tempGroup);
                        tempGroup = new Group();
                    }
                    tempGroup.Name = (nowYear + "学年" + nowSemester + "学期");
                    tempGroup.GradeItems.Add(JsonToGrade(jo));
                }
                else
                {
                    tempGroup.GradeItems.Add(JsonToGrade(jo));
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
