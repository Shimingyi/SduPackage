using System;
using System.Collections.Generic;
using System.Text;

namespace SduPackage.Model
{
    public class MyInformation
    {
        /// <summary>
        /// The name of the user
        /// </summary>
        public string myName { get; set; }

        /// <summary>
        /// The student ID of the user
        /// </summary>
        public string myStudentID { get; set; }

        /// <summary>
        /// The academy of the user
        /// </summary>
        public string myAcademy { get; set; }

        /// <summary>
        /// The specialty of the user
        /// </summary>
        public string mySpecialty { get; set; }

        /// <summary>
        /// The grade in first year
        /// </summary>
        public int firstAveGrade { get; set; }

        /// <summary>
        /// The grade in second year
        /// </summary>
        public int secondAveGrade { get; set; }

        /// <summary>
        /// The grade in third year
        /// </summary>
        public int thridAveGrade { get; set; }

        /// <summary>
        /// The grade in four year
        /// </summary>
        public int forthAveGrade { get; set; }
    }
}
