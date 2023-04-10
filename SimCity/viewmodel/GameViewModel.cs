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

        private GameModel _model;
        public string cityName; //add button and shit
        #endregion

        #region Properties
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

            //parancsok kezelése
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

            for (Int32 i = 0; i < 10; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < 10; j++)
                {
                    Fields.Add(new SimCityField
                    {
                        X = i,
                        Y = j,
                        Number = i * 100 + j, // a gomb sorszáma, amelyet felhasználunk az azonosításhoz
                        Text = String.Empty,


                    });
                }
            }
        }
            #endregion

        #region Event Methods

            private void OnNewGameSmall()
            {
                NewGameSmall?.Invoke(this, EventArgs.Empty);
            }

            #endregion

        }
    }
