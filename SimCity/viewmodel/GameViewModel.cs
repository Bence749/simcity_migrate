﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SimCity.Model;
using System.Net.Http.Headers;
using SimCity.Persistence;
using System.Runtime.ExceptionServices;

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

            _model.GameAdvanced += new EventHandler<SimCityArgsTime>(Model_AdvanceTime);
            
            //parancsok kezelése
            SpeedCommand = new DelegateCommand(param => OnSpeedChange(param));
            NewGameSmallCommand = new DelegateCommand(param => OnNewGameSmall());

            BuildCommand = new DelegateCommand(param => OnBuild(Convert.ToString(param)));


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
                field.SetZoneType = _model.Field[field.X, field.Y];

            OnPropertyChanged();
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
                        SetZoneType = new AreaType(),
                        ClickCommand = new DelegateCommand(param => ClickField(Convert.ToInt32(param))),
                    });
                }
            }
        }

        private void ClickField(Int32 index)
        {
            SimCityField field = Fields[index];

            
            switch (CurrentBuildAction)
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
                default:
                    break;
            }

            CurrentBuildAction = "";
            


            RefreshTable();
        }

        private void Model_AdvanceTime(object? sender, SimCityArgsTime e)
        {
            TimeElapsed = e.TimeElapsed;
            PopulationSum = e.Citizens;
            MoneySum = e.Money;
        }
        #endregion

        #region Event Methods
        private void OnBuild(String param)
        {
            CurrentBuildAction = param;
        }

        private void OnSpeedChange(object param) => SpeedOfGame = Convert.ToString(param);
        
        private void OnNewGameSmall()
        {
            NewGameSmall?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        }
    }
