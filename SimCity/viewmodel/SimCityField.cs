using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimCity.Persistence;

namespace SimCity.ViewModel
{
    public class SimCityField : ViewModelBase
    {
        private String _zoneType;
        private string _text = String.Empty;

        /// <summary>
        /// 0 ha lakózóna, 1 ha ipari, 2 ha kereskedelmi
        /// </summary>
        public String ZoneType
        {
            get { return _zoneType; }
            set
            {
                if (_zoneType != value)
                {
                    _zoneType = value;
                    OnPropertyChanged();
                }
            }
        }
        public String Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Vízszintes koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 X { get; set; }

        /// <summary>
        /// Függőleges koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 Y { get; set; }

        /// <summary>
        /// Sorszám lekérdezése.
        /// </summary>
        public Int32 Number { get; set; }

        /// <summary>
        /// kattintás parancs lekérdezése, vagy beállítása.
        /// </summary>
        public DelegateCommand? ClickCommand { get; set; }
    }
}

