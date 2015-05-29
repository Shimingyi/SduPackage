using System;
using System.Collections.Generic;
using System.Text;

namespace SduPackage.Model
{
    public class LessionGroup
    {
        public MyClass[] Monday { get; set; }
        public MyClass[] Tuesday { get; set; }
        public MyClass[] Wednesday { get; set; }
        public MyClass[] Thursday { get; set; }
        public MyClass[] Friday { get; set; }
        public MyClass[] Saturday { get; set; }
        public MyClass[] Sunday { get; set; }

        public LessionGroup()
        {
            Monday = new MyClass[6];
            Tuesday = new MyClass[6];
            Wednesday = new MyClass[6];
            Thursday = new MyClass[6];
            Friday = new MyClass[6];
            Saturday = new MyClass[6];
            Sunday = new MyClass[6];
            for (int i = 0; i < 6; i++)
            {
                Monday[i] = new MyClass();
                Tuesday[i] = new MyClass();
                Wednesday[i] = new MyClass();
                Thursday[i] = new MyClass();
                Friday[i] = new MyClass();
                Saturday[i] = new MyClass();
                Sunday[i] = new MyClass();
            }
        }

        public void clear()
        {
            Monday = new MyClass[6];
            Tuesday = new MyClass[6];
            Wednesday = new MyClass[6];
            Thursday = new MyClass[6];
            Friday = new MyClass[6];
            Saturday = new MyClass[6];
            Sunday = new MyClass[6];
            for (int i = 0; i < 6; i++)
            {
                Monday[i] = new MyClass();
                Tuesday[i] = new MyClass();
                Wednesday[i] = new MyClass();
                Thursday[i] = new MyClass();
                Friday[i] = new MyClass();
                Saturday[i] = new MyClass();
                Sunday[i] = new MyClass();
            }
        }
    }
}
