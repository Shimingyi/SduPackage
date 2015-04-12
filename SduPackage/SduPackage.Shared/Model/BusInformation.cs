using System;
using System.Collections.Generic;
using System.Text;

namespace SduPackage.Model
{
    public class BusInformation
    {
        public int RecNo { get; set; }

        public int id { get; set; }

        public string bus_from { get; set; }

        public string from_desc { get; set; }

        public string bus_between { get; set; }

        public string bus_to { get; set; }

        public string to_desc { get; set; }

        public string start_time { get; set; }

        public string end_time { get; set; }

        public string bus_type { get; set; }

        public string remark { get; set; }
    }
}
