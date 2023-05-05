using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SimCity.Model;
using System.Net.Http.Headers;
using SimCity.Persistence;
using System.Runtime.ExceptionServices;
using System.Windows;

namespace SimCity.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        #region Field

        private readonly GameModel _model;
        private Int32 _timeElapsed = 0;
        private Int32 _years = 0;
        private Int32 _months = 0;
        private Int32 _moneySum = 0;
        private Int32 _populationSum = 0;
        private String _currentBuildAction = String.Empty;
        private Int32 _happySum = 0;
        public string? cityName; //add button and shit
        public SimCityField _selectedField;


        #endregion

        #region Properties
        public Int32 HappySum
        {
            get => _happySum;
            set
            {
                if (_happySum != value)
                {
                    _happySum = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int32 Months
        {
            get => _months;
            set
            {
                if (_months == value) return;
                _months = value;
                OnPropertyChanged();
            }
        }

        public Int32 Years
        {
            get => _years;
            set
            {
                if (_years == value) return;
                _years = value;
                OnPropertyChanged();
            }
        }

        public Int32 TimeElapsed
        {
            get => _timeElapsed;
            set
            {
                if (_timeElapsed == value) return;
                _timeElapsed = value;
                OnPropertyChanged();
            }
        }

        public Int32 PopulationSum
        {
            get => _populationSum;
            set
            {
                if (_populationSum == value) return;
                _populationSum = value;
                OnPropertyChanged();
            }
        }

        public Int32 MoneySum
        {
            get => _moneySum;
            set
            {
                if (_moneySum == value) return;
                _moneySum = value;
                OnPropertyChanged();
            }
        }

        public String CurrentBuildAction
        {
            get => _currentBuildAction;
            set
            {
                if (_currentBuildAction == value)
                {
                    _currentBuildAction = String.Empty;
                    OnPropertyChanged();
                    return;
                }
                else
                {
                    _currentBuildAction = value;
                    OnPropertyChanged();
                }
            }
        }


        public Int32 Rows
        {
            get => _model.Field.RowSize;
        }
        public Int32 Columns
        {
            get => _model.Field.ColumnSize;
        }

        public string? SpeedOfGame
        {
            get => _model.GamePace.ToString();
            set
            {
                if ((int)_model.GamePace == Convert.ToInt32(value)) return;
                _model.GamePace = (PlaySpeed)Convert.ToInt32(value);
                OnPropertyChanged();
            }
        }
        
        public DelegateCommand SpeedCommand { get; private set; }
        public DelegateCommand BuildCommand { get; private set; }
        public DelegateCommand NewGameSmallCommand { get; private set; }
        public DelegateCommand CatastopheCommand { get; private set; }

        public DelegateCommand RaiseTaxCommand { get; private set; }
        public DelegateCommand UpgradeCommand { get; private set; }

        public DelegateCommand LowerTaxCommand { get; private set; }

        public DelegateCommand InfoCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }

        public ObservableCollection<SimCityField> Fields { get; set; }

        public ObservableCollection<MenuField> MenuItems { get; set; }

        public SimCityField SelectedField 
        {   get => _selectedField; 
            set 
            {
                _selectedField = value;
                OnPropertyChanged(); 
            }
        }

        #endregion

        #region Events

        public event EventHandler? NewGameSmall;

        /// <summary>
        /// Játékból való kilépés eseménye.
        /// </summary>
        public event EventHandler? ExitGame;

        #endregion

        #region Constructors
        public GameViewModel(GameModel model)
        {
            //játék csatlakoztatása
            _model = model;

            _model.GameAdvanced += new EventHandler<SimCityArgsTime>(Model_AdvanceTime);
            _model.GameBuild += new EventHandler<SimCityArgsClick>(Model_Build);
            
            //parancsok kezelése
            SpeedCommand = new DelegateCommand(param => OnSpeedChange(param));
            NewGameSmallCommand = new DelegateCommand(param => OnNewGameSmall());

            BuildCommand = new DelegateCommand(param => OnBuild(Convert.ToString(param)));

            InfoCommand = new DelegateCommand(param => InfoPanel());

            CatastopheCommand = new DelegateCommand(param => ForestFire());
            RaiseTaxCommand = new DelegateCommand(param => RaiseTax());
            LowerTaxCommand = new DelegateCommand(param => ReduceTax());
            UpgradeCommand = new DelegateCommand(param => UpgradeBuilding());

            ExitCommand = new DelegateCommand(param => OnExitGame());

            MenuItems = new ObservableCollection<MenuField>();
            MenuItems.Add(new MenuField { Name = "Remove", Price = null, ImageSource = "/SimCity;Component/Images/remove.png", SelectCommand = new DelegateCommand(param => OnBuild(Convert.ToString(param))), CommandParameter = "Remove" });
            MenuItems.Add(new MenuField { Name = "Road", Price = 100, ImageSource = "/SimCity;Component/Images/road.png", SelectCommand = new DelegateCommand(param => OnBuild(Convert.ToString(param))), CommandParameter = "Build Road" });
            MenuItems.Add(new MenuField { Name = "Living", Price = 0, ImageSource = "/SimCity;Component/Images/residential.jpg", SelectCommand = new DelegateCommand(param => OnBuild(Convert.ToString(param))), CommandParameter = "Build Living" });
            MenuItems.Add(new MenuField { Name = "Industrial", Price = 0, ImageSource = "/SimCity;Component/Images/industrial.jpg", SelectCommand = new DelegateCommand(param => OnBuild(Convert.ToString(param))), CommandParameter = "Build Industrial" });
            MenuItems.Add(new MenuField { Name = "Commercial", Price = 0, ImageSource = "/SimCity;Component/Images/commercial.jpg", SelectCommand = new DelegateCommand(param => OnBuild(Convert.ToString(param))), CommandParameter = "Build Commercial" });
            MenuItems.Add(new MenuField { Name = "Police", Price = 500, ImageSource = "/SimCity;Component/Images/police.jpg", SelectCommand = new DelegateCommand(param => OnBuild(Convert.ToString(param))), CommandParameter = "Build Police" });
            MenuItems.Add(new MenuField { Name = "Stadium", Price = 1000, ImageSource = "/SimCity;Component/Images/stadium.jpg", SelectCommand = new DelegateCommand(param => OnBuild(Convert.ToString(param))), CommandParameter = "Build Stadium" });
            MenuItems.Add(new MenuField { Name = "Tree", Price = 50, ImageSource = "/SimCity;Component/Images/tree10.jpg", SelectCommand = new DelegateCommand(param => OnBuild(Convert.ToString(param))), CommandParameter = "Build Tree" });
            MenuItems.Add(new MenuField { Name = "Fire Dept.", Price = 500, ImageSource = "/SimCity;Component/Images/fireDepartment.jpg", SelectCommand = new DelegateCommand(param => OnBuild(Convert.ToString(param))), CommandParameter = "Build FireDepartment" });



        }
        #endregion

        #region Private methods

        private void RefreshTable()
        {
            foreach (SimCityField field in Fields)
            {
                field.ZoneType = _model.Field[field.X, field.Y].GetAreaType();
                field.Size = _model.Field[field.X, field.Y].SizeOfZone;
                field.NumberOfResidents = _model.Field[field.X, field.Y].residents.Count;
                if(field.NumberOfResidents > 0)
                {
                    field.Happiness = _model.Field[field.X, field.Y].Happiness;
                    //field.TaxRate =; TODO
                    if (_model.Field[field.X, field.Y].SizeOfZone == SizeType.Medium)
                        field.Capacity = 25;
                    else if(_model.Field[field.X, field.Y].SizeOfZone == SizeType.Big)
                        field.Capacity = 50;
                }

                switch (field.ZoneType)
                {
                    case ("None"):
                        field.ImageSource = "/SimCity;Component/Images/background.jpg";
                        break;
                    case ("Residential"):
                        if (field.Size == SizeType.Small)
                            field.ImageSource = "/SimCity;Component/Images/residential.jpg";
                        else if (field.Size == SizeType.Medium)
                            field.ImageSource = "/SimCity;Component/Images/metropolis1.jpg";
                        else if (field.Size == SizeType.Big)
                            field.ImageSource = "/SimCity;Component/Images/metropolis2.jpg";
                        break;
                    case ("Commercial"):
                        field.ImageSource = "/SimCity;Component/Images/commercial.jpg";
                        break;
                    case ("Industrial"):
                        field.ImageSource = "/SimCity;Component/Images/industrial.jpg";
                        break;
                    case ("Road"):
                        field.ImageSource = "/SimCity;Component/Images/road.png";
                        break;
                    case ("Police"):
                        field.ImageSource = "/SimCity;Component/Images/police.jpg";
                        break;
                    case ("Stadium"):
                        field.ImageSource = "/SimCity;Component/Images/stadium.jpg";
                        break;
                    case ("Tree"):
                        var TimeElapsedSinceCreatedYears = (TimeElapsed - field.CreatedAt) / 12;
                        if (TimeElapsedSinceCreatedYears >= 0 && TimeElapsedSinceCreatedYears < 1)
                            field.ImageSource = "/SimCity;Component/Images/tree1.jpg";
                        else if (TimeElapsedSinceCreatedYears >= 1 && TimeElapsedSinceCreatedYears < 2)
                            field.ImageSource = "/SimCity;Component/Images/tree2.jpg";
                        else if (TimeElapsedSinceCreatedYears >= 2 && TimeElapsedSinceCreatedYears < 3)
                            field.ImageSource = "/SimCity;Component/Images/tree3.jpg";
                        else if (TimeElapsedSinceCreatedYears >= 3 && TimeElapsedSinceCreatedYears < 4)
                            field.ImageSource = "/SimCity;Component/Images/tree3.jpg";
                        else if (TimeElapsedSinceCreatedYears >= 4 && TimeElapsedSinceCreatedYears < 5)
                            field.ImageSource = "/SimCity;Component/Images/tree4.jpg";
                        else if (TimeElapsedSinceCreatedYears >= 5 && TimeElapsedSinceCreatedYears < 6)
                            field.ImageSource = "/SimCity;Component/Images/tree5.jpg";
                        else if (TimeElapsedSinceCreatedYears >= 6 && TimeElapsedSinceCreatedYears < 7)
                            field.ImageSource = "/SimCity;Component/Images/tree6.jpg";
                        else if (TimeElapsedSinceCreatedYears >= 7 && TimeElapsedSinceCreatedYears < 8)
                            field.ImageSource = "/SimCity;Component/Images/tree7.jpg";
                        else if (TimeElapsedSinceCreatedYears >= 8 && TimeElapsedSinceCreatedYears < 9)
                            field.ImageSource = "/SimCity;Component/Images/tree8.jpg";
                        else if (TimeElapsedSinceCreatedYears >= 9 && TimeElapsedSinceCreatedYears < 10)
                            field.ImageSource = "/SimCity;Component/Images/tree9.jpg";
                        else if (TimeElapsedSinceCreatedYears >= 10)
                            field.ImageSource = "/SimCity;Component/Images/tree10.jpg";
                        break;
                    case ("FireDepartment"):
                        field.ImageSource = "/SimCity;Component/Images/fireDepartment.jpg";
                        break;
                    default:
                        break;
                }
                if (_model.Field[field.X, field.Y].SizeOfZone == SizeType.Medium)
                {

                }
            }
            
        }

        private void RaiseTax()
        {
            if (SelectedField.TaxRate < 100)
            {
                SelectedField.TaxRate += 10;
                return;
            }
            else return;
        }

        private void ReduceTax()
        {
            if (SelectedField.TaxRate > 10)
            {
                SelectedField.TaxRate -= 10;
                return;
            }
            else return;
        }

        private void UpgradeBuilding()
        {
            if (SelectedField.Size == SizeType.Small)
                _model.Field[SelectedField.X, SelectedField.Y].SizeOfZone = SizeType.Medium;
            else if (SelectedField.Size == SizeType.Medium)
                _model.Field[SelectedField.X, SelectedField.Y].SizeOfZone = SizeType.Big;
            RefreshTable();
            return;
        }

        //az infopanel mindig megállítja a játékot, majd 1-es sebességen (Normal) indítja újra, ha bezárul a felugró ablak
        private void InfoPanel()
        {
            //string tmpSpeed = SpeedOfGame;
            SpeedOfGame = Convert.ToString(0);
            MessageBox.Show("SimCity, a városépítő játék\n\n\nPénz: Ebből tudod finanszírozni nagyszerű városod fejlesztését.\n\nElégedettség: Megmutatja mennyire elégíted ki a lakosaid igényeit. Meghatározza a betelepülés sebességét. Ha túl alacsonra esne, elveszíted a játékot.\n\nLakosság: A városodban élő polgárok. Adót fizetnek, évente. Maguktól jönnek a városba és keresnek munkát, amennyiben van szabad munka- és lakóhely számukra.\n\nIdő: Az eltelt évek száma.\n\nSebesség: a szimuláció sebessége.\nStop: a játék áll. \nNormal: 1 másodperc = 1 hónap. \nFast: 1 másodperc = 2 hónap. \nFaster: 1 másodperc = 4 hónap", "SimCity", MessageBoxButton.OK, MessageBoxImage.Information);
            //SpeedOfGame = tmpSpeed;
            SpeedOfGame = Convert.ToString(1);
        }

        //a tűzvész 20% eséllyel Lebontja az egyes erdőket
        private void ForestFire()
        {
            Random rnd = new Random();
            foreach (SimCityField field in Fields)
            {
                if(field.ZoneType == "Tree") 
                {
                    if(rnd.Next(1,5) == 1)
                    {
                       _model.ClickHandle(field.X, field.Y, "Remove"); //remove 'building' on field
                    }
                }
            }

            RefreshTable();
        }
        private void GenerateTable()
        {
            Fields.Clear();

            for (Int32 i = 0; i < _model.Field.RowSize; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < _model.Field.ColumnSize; j++)
                {
                    Fields.Add(new SimCityField
                    {
                        X = i,
                        Y = j,
                        Number = i * _model.Field.RowSize + j, // a gomb sorszáma, amelyet felhasználunk az azonosításhoz
                        Text = String.Empty,
                        ZoneType = new AreaType().GetAreaType(),
                        ClickCommand = new DelegateCommand(param => ClickField(Convert.ToInt32(param))),
                        NumberOfResidents = new AreaType().residents.Count,
                        MaintanenceCost = new AreaType().MaintenanceCost,
                        TaxRate = 10,
                        Happiness = new AreaType().Happiness,
                        Capacity = 15, //starts at 15, aka SMALL
                        RemovePrice = new AreaType().RemovePrice,
                    }) ;
                }
            }
        }

        private void ConvertTime()
        {
            Months = TimeElapsed;
            Years = Months / 12;
            Months = Months % 12;
        }
        private void CountHappySum()
        {
            HappySum = 0;
            foreach (SimCityField field in Fields)
            {
                HappySum += field.Happiness;
            }
        }
        private void ClickField(Int32 index)
        {
            if (CurrentBuildAction != "")
            {
                SimCityField field = Fields[index];
                var paramsToClick = CurrentBuildAction.Split(' ');

                if (paramsToClick[0] == "Remove")
                {
                    _model.ClickHandle(field.X, field.Y, paramsToClick[0]);
                    field.CreatedAt = 0;
                }
                else if (Fields[index].ZoneType == "Residential" || Fields[index].ZoneType == "Commercial" || Fields[index].ZoneType == "Industrial")
                {
                    
                    SelectedField = field;
                    /*SpeedOfGame = Convert.ToString(0);
                    MessageBox.Show(field.ZoneType + " Zone" + 
                        Environment.NewLine + "Lakosok: " + field.NumberOfResidents + 
                        Environment.NewLine + "Kapacitás: " + field.Capacity +
                        Environment.NewLine + "Elégedettség: " + field.Happiness +
                        Environment.NewLine + "Fentartási költség: " + field.MaintanenceCost +
                        Environment.NewLine + "Bontási kompenzáció: " + field.RemovePrice +
                        Environment.NewLine + "Adó: " + "TO BE IMPLEMENTED"
                        , "SimCity", MessageBoxButton.OK, MessageBoxImage.Information);
                    SpeedOfGame = Convert.ToString(1);*/
                    RefreshTable();
                }
                else
                    switch (paramsToClick[1])
                    {
                        case "Road":
                            _model.ClickHandle(field.X, field.Y, "Build", new Road());
                            if (field.ZoneType == "None")
                                field.CreatedAt = TimeElapsed;
                            break;
                        case "Living":
                            _model.ClickHandle(field.X, field.Y, "Build", new ResidentialZone());
                            if (field.ZoneType == "None")
                                field.CreatedAt = TimeElapsed;
                            break;
                        case "Commercial":
                            _model.ClickHandle(field.X, field.Y, "Build", new CommercialZone());
                            if (field.ZoneType == "None")
                                field.CreatedAt = TimeElapsed;
                            break;
                        case "Industrial":
                            _model.ClickHandle(field.X, field.Y, "Build", new IndustrialZone());
                            if (field.ZoneType == "None")
                                field.CreatedAt = TimeElapsed;
                            break;
                        case "Police":
                            _model.ClickHandle(field.X, field.Y, "Build", new Police());
                            if (field.ZoneType == "None")
                                field.CreatedAt = TimeElapsed;
                            break;
                        case "Stadium":
                            _model.ClickHandle(field.X, field.Y, "Build", new Stadium());
                            if (field.ZoneType == "None")
                                field.CreatedAt = TimeElapsed;
                            break;
                        case "Tree":
                            _model.ClickHandle(field.X, field.Y, "Build", new Tree());
                            if (field.ZoneType == "None")
                                field.CreatedAt = TimeElapsed;
                            break;
                        case "FireDepartment":
                            _model.ClickHandle(field.X, field.Y, "Build", new FireDepartment());
                            if (field.ZoneType == "None")
                                field.CreatedAt = TimeElapsed;
                            break;
                    }
                
                RefreshTable();
            }
            else if (Fields[index].ZoneType == "Residential" || Fields[index].ZoneType == "Commercial" || Fields[index].ZoneType == "Industrial")
            {
                SimCityField field = Fields[index];
                SelectedField = field;
                /*SpeedOfGame = Convert.ToString(0);
                MessageBox.Show(field.ZoneType + " Zone" + 
                    Environment.NewLine + "Lakosok: " + field.NumberOfResidents + 
                    Environment.NewLine + "Kapacitás: " + field.Capacity +
                    Environment.NewLine + "Elégedettség: " + field.Happiness +
                    Environment.NewLine + "Fentartási költség: " + field.MaintanenceCost +
                    Environment.NewLine + "Bontási kompenzáció: " + field.RemovePrice +
                    Environment.NewLine + "Adó: " + "TO BE IMPLEMENTED"
                    , "SimCity", MessageBoxButton.OK, MessageBoxImage.Information);
                SpeedOfGame = Convert.ToString(1);*/
                RefreshTable();
            }
        }

        #endregion

        #region Event Methods

        public void View_CreateGame(Int32 row, Int32 col)
        {
            _model.CreateGame(row, col);
            
            Fields = new ObservableCollection<SimCityField>();
            GenerateTable();
            RefreshTable();
        }
        private void Model_AdvanceTime(object? sender, SimCityArgsTime e)
        {
            TimeElapsed = e.TimeElapsed;
            PopulationSum = e.Citizens;
            MoneySum = e.Money;
            CountHappySum();
            ConvertTime();
            RefreshTable();
        }
        
        private void Model_Build(object? sender, SimCityArgsClick e)
        {
            if (e.Money is not null)
            {
                MoneySum = (Int32)e.Money;
                
                //TODO: Sound in case of good action
            }
            else
            {
                //TODO: Sound in case of bad action
            }
            
            RefreshTable();
        }
        private void OnBuild(String param)
        {
            CurrentBuildAction = param;
        }


        private void OnSpeedChange(object param) => SpeedOfGame = Convert.ToString(param);
        
        private void OnNewGameSmall()
        {
            NewGameSmall?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        }
    }
