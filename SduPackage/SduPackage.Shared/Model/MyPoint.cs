using System;
using System.Collections.Generic;
using System.Text;

namespace SduPackage.Model
{
    public class MyPoint
    {
        /// <summary>
        /// The name of this class
        /// </summary>
        public string className { get; set; }

        /// <summary>
        /// The year of this class
        /// </summary>
        public int classYear { get; set; }

        /// <summary>
        /// The semester of the class, up semester is 1, down semester is 0
        /// </summary>
        public int classSemester { get; set; }

        /// <summary>
        /// The credit of this class
        /// </summary>
        public string classCredit { get; set; }

        /// <summary>
        /// The attitude of this class, for example:Must study, choose study
        /// </summary>
        public string classAttitude { get; set; }

        /// <summary>
        /// The grade of this class
        /// </summary>
        public string classGrade { get; set; }

        /// <summary>
        /// The time of the test of this class
        /// </summary>
        public string examTime { get; set; }
    }
}
