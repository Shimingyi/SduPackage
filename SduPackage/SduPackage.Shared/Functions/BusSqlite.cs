using SduPackage.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace SduPackage.Functions
{
    public class BusSqlite
    {
        SQLiteConnection conn;
        SQLiteCommand cmd;
        string[] NumToCampus;

        public BusSqlite()
        {
            preprationDic();
            conn = GetConn();
            cmd = new SQLiteCommand(conn);
        }

        #region 方法
        public List<BusInformation> SearchBus(int startNum,int endNum)
        {
            List<BusInformation> _BusInformations = null;
            if (CheckSummer())
            {
                _BusInformations = SearchBusWithTime(startNum, endNum, "summer_time");
            }
            else
            {
                _BusInformations = SearchBusWithTime(startNum, endNum, "winter_time");
            }
            return _BusInformations;
        }

        #endregion

        #region 私有方法
        private void preprationDic()
        {
            NumToCampus = new string[7];
            NumToCampus[0] = null;
            NumToCampus[1] = "中心校区";
            NumToCampus[2] = "洪家楼校区";
            NumToCampus[3] = "千佛山校区";
            NumToCampus[4] = "兴隆山校区";
            NumToCampus[5] = "软件园校区";
            NumToCampus[6] = "兴隆山校区";
        }

        private SQLiteConnection GetConn()
        {
            return new SQLiteConnection(ApplicationData.Current.LocalFolder.Path + "\\bus.db");
        }

        private bool CheckSummer()
        {
            bool isSummer = true;

            DateTime now = DateTime.Now;
            int nowNum = (now.Month * 100 + now.Day);

            if ( (nowNum > 1008)&&(nowNum < 430))
            {
                isSummer = false;
            }

            return isSummer;
        }

        private List<BusInformation> SearchBusWithTime(int startNum, int endNum, string TableName)
        {
            List<BusInformation> _BusInformations = new List<BusInformation>();

            //string sql = ("select * from search_table, summer_time where search_table.id = summer_time.id and search_table.id in(select id from search_table where RecNo = '1')");
            //string sql = ("select * from search_table, "+TableName+" where search_table.id ="+TableName+".id and search_table.id in (select id from search_table where from_ = "+NumToCampus[startNum]+" and to_ = "+NumToCampus[endNum]+")");
            string sql = ("select id from search_table where from_ = '"+NumToCampus[startNum]+"' and to_ = '"+NumToCampus[endNum]+"'");
            cmd.CommandText = sql;
            List<ID> busIdSet = cmd.ExecuteQuery<ID>();

            for (int i = 0; i < busIdSet.Count;i++ )
            {
                sql = ("select * from "+TableName+" where id = " + busIdSet[i].id);
                cmd.CommandText = sql;
                List<BusInformation> one = cmd.ExecuteQuery<BusInformation>();
                _BusInformations.Add(one[0]);
            }
            return _BusInformations;
        }
        #endregion

    }
}
