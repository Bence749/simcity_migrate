using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SimCity.Persistence;

/// <summary>
/// Enum <c>PlaySpeed</c> define the pace of the game.
/// <param name="Stop">The game will not advance</param>
/// <param name="Normal">1 real life second = 1 hours in-game</param>
/// <param name="Fast">1 real life second = 2 hours in-game</param>
/// <param name="Faster">1 real life second = 4 hours in-game</param>
/// </summary>
public enum PlaySpeed { Stop, Normal, Fast, Faster }

namespace SimCity.Model
{
    public class GameModel
    {
        private Map _field = null!;
        private Int32 _timeElapsed;
        private Int32 _tickCount;
        private Int32 _money;
        private Int32 _citizens;

        public PlaySpeed GamePace { get; set; }

        public Map Field => _field;

        public event EventHandler<SimCityArgsTime>? GameAdvanced;
        public event EventHandler<SimCityArgsClick>? GameBuild;

        public GameModel()
        {
            _timeElapsed = 0;
            _money = 100000;
            _citizens = 0;
            GamePace = PlaySpeed.Normal;
        }

        public void CreateGame(Int32 rows, Int32 columns)
        {
            _field = new Map(rows, columns);
        }

        public void AdvanceTime()
        {
            ++_tickCount;
            if (GamePace != PlaySpeed.Stop &&  (GamePace == PlaySpeed.Faster || 
                                                (GamePace == PlaySpeed.Fast && _tickCount % 2 == 0) ||
                                                (GamePace == PlaySpeed.Normal && _tickCount % 4 == 0)))
            {
                ++_timeElapsed;
                _money -= _field.GetMaintenance();
                
                //Residential area ticks
                if (_field.NumberOfCitizens < _field.MaxCitizens)
                {
                    //TODO: Residents more likely to fill happier zones
                    List<(Int32, Int32)> residentialZones = _field.AvailableZones("Residential").Where(y => 
                        _field[y.Item2.Item1, y.Item2.Item2].NumberOfResidents
                        < (Int32) _field[y.Item2.Item1, y.Item2.Item2].SizeOfZone)
                        .SelectMany(y => Enumerable.Repeat(y.Item2, y.Item1.Happiness)).ToList();

                    if (residentialZones.Count != 0)
                    {
                        Random randSelector = new Random();
                        (Int32, Int32) selectedField =
                            residentialZones.ElementAt(randSelector.Next(residentialZones.Count));
                        _field[selectedField.Item1, selectedField.Item2].NumberOfResidents += 1;
                    }
                }
            }

            this.GameAdvanced?.Invoke(this, new SimCityArgsTime(_timeElapsed, _field.NumberOfCitizens, _money));
        }
        
        /// <summary>
        /// Handling <c>ClickEvent</c> from the <c>ViewModel</c> and  the requested child of
        /// <see cref="AreaType" /> if possible. In case the 
        /// </summary>
        /// <param name="row">Row of clicked cell.</param>
        /// <param name="column">Column of the cell.</param>
        /// <param name="mode">Switch whether it's need to build or remove.
        /// Valid values are "Build" and "Remove"</param>
        /// <param name="toBuild">Build the requested child of <see cref="AreaType"/>.</param>
        /// <remarks>
        /// This method performs the appropriate action based on the <paramref name="mode"/> parameter. 
        /// If <paramref name="mode"/> is "Build", the <paramref name="toBuild"/> parameter is used to construct a building on the clicked cell.
        /// If <paramref name="mode"/> is "Remove", the building on the clicked cell is removed and its cost is added to the player's money.
        /// This method also raises the <see cref="GameBuild"/> event with a <see cref="SimCityArgsClick"/> object containing the updated money amount.
        /// In case the action is cannot be performed <see cref="SimCityArgsClick" /> money variable will be null.
        /// </remarks>
        public async void ClickHandle(Int32 row, Int32 column, String mode, AreaType toBuild = null!)
        {
            await Task.Run(() =>
                {
                    try
                    {
                        switch (mode)
                        {
                            case "Build":
                                Int32 cost = _field.Build(row, column, toBuild);
                                _money -= cost;
                                break;
                            case "Remove":
                                Int32 prize = _field.Remove(row, column);
                                _money += prize;
                                break;
                        }

                        GameBuild?.Invoke(this, new SimCityArgsClick(_money));
                    }
                    catch (PersistenceExceptions e)
                    {
                        GameBuild?.Invoke(this, new SimCityArgsClick(null));
                    }
                });
        }
    }
}
