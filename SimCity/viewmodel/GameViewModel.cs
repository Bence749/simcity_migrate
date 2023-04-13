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
        private Int32 _moneySum = 0;
        private Int32 _populationSum = 0;
        private String _currentBuildAction = String.Empty;
        public string? cityName; //add button and shit


        #endregion

        #region Properties

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
                if (_currentBuildAction == value) return;
                _currentBuildAction = value;
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


        public DelegateCommand InfoCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }


        public ObservableCollection<SimCityField> Fields { get; set; }

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

            ExitCommand = new DelegateCommand(param => OnExitGame());



            // játéktábla létrehozása
            Fields = new ObservableCollection<SimCityField>();
            GenerateTable();
            RefreshTable();

        }
        #endregion

        #region Private methods

        private void RefreshTable()
        {
            foreach (SimCityField field in Fields) // inicializálni kell a mezőket is
                field.ZoneType = _model.Field[field.X, field.Y].GetAreaType();
            
        }

        //az infopanel mindig megállítja a játékot, majd 1-es sebességen (Normal) indítja újra, ha bezárul a felugró ablak
        private void InfoPanel()
        {
            //string tmpSpeed = SpeedOfGame;
            SpeedOfGame = Convert.ToString(0);
            MessageBox.Show("Dikh, itt egy lorem ipsum \n\nSimCity, a városépítő játék\n\n\nPénz: Ebből tudod finanszírozni nagyszerű városod fejlesztését.\n\nElégedettség: Megmutatja mennyire elégíted ki a lakosaid igényeit. Meghatározza a betelepülés sebességét. Ha túl alacsonra esne, elveszíted a játékot.\n\nLakosság: A városodban élő polgárok. Adót fizetnek, évente. Maguktól jönnek a városba és keresnek munkát, amennyiben van szabad munka- és lakóhely számukra.\n\nIdő: Az eltelt évek száma.\n\nSebesség: a szimuláció sebessége.\nStop: a játék áll. \nNormal: 1 másodperc = 1 év. \nFast: 1 másodperc = 2 év. \nFaster: 1 másodperc = 4 év", "SzimSziti", MessageBoxButton.OK, MessageBoxImage.Information);
            //SpeedOfGame = tmpSpeed;
            SpeedOfGame = Convert.ToString(1);
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
                        ZoneType= new AreaType().GetAreaType(),
                        ClickCommand = new DelegateCommand(param => ClickField(Convert.ToInt32(param))),
                    });
                }
            }
        }

        private void ClickField(Int32 index)
        {
            if (CurrentBuildAction != "")
            {
                SimCityField field = Fields[index];
                var paramsToClick = CurrentBuildAction.Split(' ');

                if (paramsToClick[0] == "Remove")
                    _model.ClickHandle(field.X, field.Y, paramsToClick[0]);
                else
                    switch (paramsToClick[1])
                    {
                        case "Road":
                            _model.ClickHandle(field.X, field.Y, "Build", new Road());
                            break;
                        case "Living":
                            _model.ClickHandle(field.X, field.Y, "Build", new ResidentialZone());
                            break;
                        case "Commercial":
                            _model.ClickHandle(field.X, field.Y, "Build", new CommercialZone());
                            break;
                        case "Industrial":
                            _model.ClickHandle(field.X, field.Y, "Build", new IndustrialZone());
                            break;
                        case "Police":
                            _model.ClickHandle(field.X, field.Y, "Build", new Police());
                            break;
                        case "Stadium":
                            _model.ClickHandle(field.X, field.Y, "Build", new Stadium());
                            break;
                        case "Tree":
                            _model.ClickHandle(field.X, field.Y, "Build", new Tree());
                            break;
                    }

                CurrentBuildAction = "";
                
                RefreshTable();
            }
        }

        #endregion

        #region Event Methods
        private void Model_AdvanceTime(object? sender, SimCityArgsTime e)
        {
            TimeElapsed = e.TimeElapsed;
            PopulationSum = e.Citizens;
            MoneySum = e.Money;
        }
        
        private void Model_Build(object? sender, SimCityArgsClick e)
        {
            MoneySum = e.Money;
            
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
