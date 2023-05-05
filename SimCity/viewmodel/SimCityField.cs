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
        private int _numberOfResidents;
        private int _maintanenceCost;
        private int _removePrice;
        private int _capacity; //based on model.SizeType
        private int _happiness;
        private string _imageSource = String.Empty;
        private SizeType _size;
        private int _createdAt;
        
        //TODO
        private int _taxRate;

        public string ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    OnPropertyChanged();
                }
            }
        }
        public SizeType Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnPropertyChanged();
                }
            }
        }
        public int CreatedAt
        {
            get { return _createdAt; }
            set
            {
                if (value != _createdAt)
                { 
                    _createdAt = value;
                    OnPropertyChanged();
                }
            }
        }

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
        public Int32 TaxRate
        {
            get { return _taxRate; }
            set
            {
                if (_taxRate != value)
                {
                    _taxRate = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int32 Happiness
        {
            get { return _happiness; }
            set
            {
                if (_happiness != value)
                {
                    _happiness = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int32 Capacity
        {
            get { return _capacity; }
            set
            {
                if (_capacity != value)
                {
                    _capacity = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int32 RemovePrice
        {
            get { return _removePrice; }
            set
            {
                if (_removePrice != value)
                {
                    _removePrice = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int32 MaintanenceCost
        {
            get { return _maintanenceCost; }
            set
            {
                if (_maintanenceCost != value)
                {
                    _maintanenceCost = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int32 NumberOfResidents
        {
            get { return _numberOfResidents; }
            set
            {
                if (_numberOfResidents != value)
                {
                    _numberOfResidents = value;
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

