using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SimCity.ViewModel
{
    public class MenuField : ViewModelBase
    {

        public String Name { get; set; }
        public Int32? Price { get; set; }
        public String ImageSource { get; set; }
        public String CommandParameter { get; set; }
        public DelegateCommand SelectCommand { get; set; }
    }
}

