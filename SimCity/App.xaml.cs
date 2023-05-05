using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Win32;
using SimCity.View;
using SimCity.ViewModel;
using SimCity.Model;
using SimCity.viewmodel;

namespace SimCity
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private GameModel _model = null!;
        private GameViewModel _viewModel = null!;
        private WelcomeWindowViewModel _wwviewModel = null!;
        private WelcomeWindow? _welcomeView = null!;
        private MainWindow? _view;
        private DispatcherTimer _timer = null!;

        #endregion

        #region Constructors

        public App()
        {
            Startup += new StartupEventHandler(Welcome);
        }

        #endregion

        #region Application event handlers

        private void Welcome(object sender, StartupEventArgs e) 
        {
            _wwviewModel = new WelcomeWindowViewModel();
            _wwviewModel.ExitGame += new EventHandler(WViewModel_ExitGame);
            _wwviewModel.NewGameSmall += new EventHandler(ViewModel_StartGame);
            _welcomeView = new WelcomeWindow();
            _welcomeView.DataContext = _wwviewModel;

            _welcomeView.Show();
        }

        private void App_Startup(object? sender, EventArgs e)
        {
            
            // modell létrehozás
            _model = new GameModel();

            // nézemodell létrehozása
            _viewModel = new GameViewModel(_model);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);

            // nézet létrehozása
            _view = new MainWindow();
            
            // Get the size of the defined grid and create the model accordingly
            var itemPanel = (ItemsPanelTemplate?)_view.FindName("GameField");
            if (itemPanel is not null)
            {
                var gridContent = (UniformGrid)itemPanel.LoadContent();
                _viewModel.View_CreateGame(gridContent.Rows, gridContent.Columns);
            }
            
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing); // eseménykezelés a bezáráshoz
            _view.Show();


            
            

            // időzítő létrehozása
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Start();
            _welcomeView.Close();
        }

        #endregion
        
        private void View_Closing(object? sender, CancelEventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            if (MessageBox.Show("Are you sure, you want to quit?", "SimCity", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; // töröljük a bezárást

                if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                    _timer.Start();
            }
        }

        #region ViewModel Event Handlers
        /// <summary>
        /// Játékból való kilépés eseménykezelője.
        /// </summary>
        private void ViewModel_ExitGame(object? sender, System.EventArgs e)
        {
            _view.Close(); // ablak bezárása
        }

        private void WViewModel_ExitGame(object? sender, System.EventArgs e)
        {
            _welcomeView.Close();
        }

        private void ViewModel_StartGame(object? sender, System.EventArgs e)
        {
            App_Startup(sender, e);
        }
        #endregion

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _model.AdvanceTime();
        }

    }
}