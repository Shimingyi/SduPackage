using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SduPackage.Model
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

        public ObservableCollection<News> NewsItems
        {
            get;
            private set;
        }

        public ObservableCollection<MyPoint> GradeItems
        {
            get;
            private set;
        }

        public Group()
        {
            this.NewsItems = new ObservableCollection<News>();
            this.GradeItems = new ObservableCollection<MyPoint>();
        }
    }
}
