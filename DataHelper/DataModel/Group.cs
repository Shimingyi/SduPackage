using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHelper.DataModel
{
    public class Group 
    {
        private string _name = string.Empty;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        public ObservableCollection<News> Items
        {
            get;
            private set;
        }

        public Group()
        {
            this.Items = new ObservableCollection<News>();
        }
    }
}
