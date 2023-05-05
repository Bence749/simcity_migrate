using SimCity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimCity.viewmodel
{
    public class WelcomeWindowViewModel : ViewModelBase
    {
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand NewGameSmallCommand { get; set; }

        public event EventHandler? NewGameSmall;

        /// <summary>
        /// Játékból való kilépés eseménye.
        /// </summary>
        public event EventHandler? ExitGame;

        public WelcomeWindowViewModel() 
        {
            NewGameSmallCommand = new DelegateCommand(param => OnNewGameSmall());
            ExitCommand = new DelegateCommand(param => OnExitGame());
        }


        private void OnNewGameSmall()
        {
            NewGameSmall?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

    }
}
