using System;
using System.Collections.Generic;
using System.Text;

namespace SduPackage.Model
{
    public class CardRecord
    {
        /// <summary>
        /// The shopping time of this record
        /// </summary>
        public string c_time { get; set; }

        /// <summary>
        /// Where you do shopping in thus record
        /// </summary>
        public string c_place { get; set; }

        /// <summary>
        /// What the type of this record
        /// </summary>
        public string c_type { get; set; }

        /// <summary>
        /// The money value of this record
        /// </summary>
        public string c_value { get; set; }
    }
}
