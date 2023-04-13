using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimCity.Persistence;

/// <summary>
/// Enum <c>PlaySpeed</c> define the pace of the game.
/// <param name="Stop">The game will not advance</param>
/// <param name="Normal">1 real life second = 1 hours in-game</param>
/// <param name="Fast">1 real life second = 2 hours in-game</param>
/// <param name="FastProMax">1 real life second = 4 hours in-game</param>
/// </summary>
public enum PlaySpeed { Stop, Normal, Fast, Faster }

namespace SimCity.Model
{
    public class GameModel
    {
        private Map _fields = null!;
        private Int32 _timeElapsed;
        private Int32 _tickCount;
        private Int32 _money;
        private Int32 _citizens;

        public PlaySpeed GamePace { get; set; }

        public event EventHandler<SimCityArgs>? GameAdvanced;

        public GameModel()
        {
            _timeElapsed = 0;
            _money = 100000;
            _citizens = 0;
            GamePace = PlaySpeed.Normal;
        }

        public void CreateGame(Int32 rows, Int32 columns)
        {
            _fields = new Map(rows, columns);
        }

        public void AdvanceTime()
        {
            ++_tickCount;
            if (GamePace != PlaySpeed.Stop &&  (GamePace == PlaySpeed.Faster || 
                                                (GamePace == PlaySpeed.Fast && _tickCount % 2 == 0) ||
                                                (GamePace == PlaySpeed.Normal && _tickCount % 4 == 0)))
            {
                ++_timeElapsed;
                _citizens += 10;
                _money += _citizens * 10;
            }

            this.GameAdvanced?.Invoke(this, new SimCityArgs(_timeElapsed, _citizens, _money));
        }
        
        public void ClickHandle(Int32 row, Int32 column, String mode, AreaType toBuild = AreaType.None)
        {
            Int32 cost = 0;
            switch (mode)
            {
                case "Build": cost = _fields.Build(row, column, toBuild);
                    break;
                case "Remove": _fields.Remove(row, column);
                    break;
            }

            _money -= cost;
        }
    }
}
