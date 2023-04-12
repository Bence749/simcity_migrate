using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Enum <c>PlaySpeed</c> define the pace of the game.
/// <param name="Stop">The game will not advance</param>
/// <param name="Normal">1 real life second = 1 hours in-game</param>
/// <param name="Fast">1 real life second = 2 hours in-game</param>
/// <param name="FastProMax">1 real life second = 4 hours in-game</param>
/// </summary>
public enum PlaySpeed { Stop, Normal, Fast, FastProMax }

namespace SimCity.Model
{
    public class GameModel
    {
        private Int32 _time;
        private Int32 _tickCount;
        private PlaySpeed _playSpeed;
        
        
        public GameModel()
        {
            _playSpeed = PlaySpeed.Normal;
        }

        public void AdvanceTime()
        {
            ++_tickCount;
            if (_playSpeed != PlaySpeed.Stop &&  (_playSpeed == PlaySpeed.FastProMax || 
                                                 (_playSpeed == PlaySpeed.Fast && _tickCount % 2 == 0) ||
                                                 (_playSpeed == PlaySpeed.Normal && _tickCount % 4 == 0)))
            {
                //TODO: AdvanceTime() action
            }
        }
    }
}
