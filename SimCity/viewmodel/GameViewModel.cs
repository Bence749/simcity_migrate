using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SimCity.Model;

namespace SimCity.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        #region Field

        private readonly GameModel _model;
        private Int32 _timeElapsed = 0;
        private Int32 _moneySum = 0;
        private Int32 _populationSum = 0;
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
        public DelegateCommand NewGameSmallCommand { get; private set; }

        public ObservableCollection<SimCityField> Fields { get; set; }

        #endregion

        #region Events

        public event EventHandler? NewGameSmall;

        #endregion

        #region Constructors
        public GameViewModel(GameModel model)
        {
            //játék csatlakoztatása
            _model = model;

            _model.GameAdvanced += new EventHandler<SimCityArgs>(Model_AdvanceTime);
            
            //parancsok kezelése
            SpeedCommand = new DelegateCommand(param => OnSpeedChange(param));
            NewGameSmallCommand = new DelegateCommand(param => OnNewGameSmall());


            // játéktábla létrehozása
            Fields = new ObservableCollection<SimCityField>();
            GenerateTable();
            RefreshTable();

        }
        #endregion

        #region Private methods

        private void RefreshTable()
        {
            return;
        }

        private void GenerateTable()
        {
            Fields.Clear();

            for (Int32 i = 0; i < 50; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < 50; j++)
                {
                    Fields.Add(new SimCityField
                    {
                        X = i,
                        Y = j,
                        Number = i * 100 + j, // a gomb sorszáma, amelyet felhasználunk az azonosításhoz
                        Text = String.Empty,
                        ZoneType = 0

                    });
                }
            }
        }

        private void Model_AdvanceTime(object? sender, SimCityArgs e)
        {
            TimeElapsed = e.TimeElapsed;
            PopulationSum = e.Citizens;
            MoneySum = e.Money;
        }
        #endregion

        #region Event Methods

        private void OnSpeedChange(object param) => SpeedOfGame = Convert.ToString(param);
        
        private void OnNewGameSmall()
        {
            NewGameSmall?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        }
    }
