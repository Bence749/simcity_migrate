﻿using System;
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
public enum PlaySpeed { Stop, Normal, Fast, Faster }

namespace SimCity.Model
{
    public class GameModel
    {
        private Int32 _timeElapsed;
        private Int32 _tickCount;

        public PlaySpeed GamePace { get; set; }

        public event EventHandler<SimCityArgs>? GameAdvanced;
        
        public GameModel()
        {
            _timeElapsed = 0;
            GamePace = PlaySpeed.Normal;
        }

        public void AdvanceTime()
        {
            ++_tickCount;
            if (GamePace != PlaySpeed.Stop &&  (GamePace == PlaySpeed.Faster || 
                                                 (GamePace == PlaySpeed.Fast && _tickCount % 2 == 0) ||
                                                 (GamePace == PlaySpeed.Normal && _tickCount % 4 == 0)))
            {
                ++_timeElapsed;
            }

            this.GameAdvanced?.Invoke(this, new SimCityArgs(_timeElapsed));
        }
    }
}