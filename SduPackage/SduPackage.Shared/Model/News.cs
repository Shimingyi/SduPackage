using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SduPackage.Model
{
    public class News
    {
        /// <summary>
        /// 板块id，用于区分是评论还是通知等类别
        /// </summary>
        public int blockId { get; set; }

        /// <summary>
        /// 每篇文章在数据库有一个单独的id
        /// </summary>
        public int pid { get; set; }

        /// <summary>
        /// 文章等级
        /// </summary>
        public int priorty { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        public string addTime { get; set; }

        /// <summary>
        /// 文章编辑时间
        /// </summary>
        public string editTime { get; set; }

        /// <summary>
        /// 文章发布时间
        /// </summary>
        public string subTitle { get; set; }

        /// <summary>
        /// 文章作者
        /// </summary>
        public string editor { get; set; }

        /// <summary>
        /// 文章记者
        /// </summary>
        public string reporter { get; set; }

        /// <summary>
        /// 文章来源
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        public string keyWord { get; set; }


    }
}
