using System;
using System.Collections.Generic;
using System.Text;

namespace SduPackage.Model
{
    public class BookInformation
    {
        //Book user information
        public string b_id { get; set; }
        public string b_title { get; set; }
        public string b_data { get; set; }


        //Book search information
        public string b_total { get; set; }
        public string b_detailurl { get; set; }
        public string b_number { get; set; }
        public string b_canborrow { get; set; }

        //Book browse state
        public string b_location { get; set; }
        public string b_state { get; set; }

        
    }
}
