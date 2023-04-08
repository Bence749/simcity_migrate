using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using SimCity.View;
using SimCity.ViewModel;
using SimCity.Model;

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
        private MainWindow _view = null!;
        private DispatcherTimer _timer = null!;

        #endregion

        #region Constructors

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        #endregion

        #region Application event handlers

        private void App_Startup(object? sender, StartupEventArgs e)
        {
            // nézemodell létrehozása
            _viewModel = new GameViewModel(_model);

            // nézet létrehozása
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing); // eseménykezelés a bezáráshoz
            _view.Show();

            // időzítő létrehozása
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Start();
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

        private void Timer_Tick(object? sender, EventArgs e)
        {
            //_model.AdvanceTime();
        }

    }
}