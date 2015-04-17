using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SduPackage.Model
{
    public class User
    {
        #region 属性
        /// <summary>
        /// The name of User
        /// </summary>
        public string myName { get; set; }

        /// <summary>
        /// The stuNum of User
        /// </summary>
        public string myStudentID { get; set; }

        /// <summary>
        /// The academy of User like '软件学院'
        /// </summary>
        public string myAcademy { get; set; }

        /// <summary>
        /// The academy of User like '软件工程'
        /// </summary>
        public string mySpecialty { get; set; }

        /// <summary>
        /// The grade of first academy
        /// </summary>
        public string firstAveGrade { get; set; }

        /// <summary>
        /// The grade of second academy
        /// </summary>
        public string secondAveGrade { get; set; }

        /// <summary>
        /// The grade of third academy
        /// </summary>
        public string thirdAveGrade { get; set; }

        /// <summary>
        /// The grade of four academy
        /// </summary>
        public string forthAveGrade { get; set; }
        #endregion
    }
}
