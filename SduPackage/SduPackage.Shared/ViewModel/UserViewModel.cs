using Newtonsoft.Json.Linq;
using SduPackage.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Windows.Storage;

namespace SduPackage.ViewModel
{
    public class UserViewModel
    {
        #region 字段
        Windows.Storage.StorageFolder _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        #endregion

        #region 属性
        public User user
        {
            get;
            private set;
        }
        #endregion

        #region 构造方法
        public UserViewModel()
        {
            this.user = new User();
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
            StorageFile onlineNews = await _localFolder.GetFileAsync(filename);
            string result = await FileIO.ReadTextAsync(onlineNews);
            FileTxtToUser(result);
        }

        private void FileTxtToUser(string result)
        {
            JObject jo = JObject.Parse(result);
            user.myName = jo["myName"].ToString();
            user.myStudentID = jo["myStudentID"].ToString();
            user.myAcademy = jo["myAcademy"].ToString();
            user.mySpecialty = jo["mySpecialty"].ToString();
            user.firstAveGrade = jo["firstAveGrade"].ToString();
            user.secondAveGrade = jo["secondAveGrade"].ToString();
            user.thirdAveGrade = jo["thridAveGrade"].ToString();
            user.forthAveGrade = jo["forthAveGrade"].ToString();
        }
        #endregion
    }
}
