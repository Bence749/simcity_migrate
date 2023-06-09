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
    public class LimitedQueue<T> : Queue<T>
    {
        private readonly int _maxSize;

        public LimitedQueue(int maxSize)
        {
            _maxSize = maxSize;
        }

        public new void Enqueue(T item)
        {
            while (this.Count >= _maxSize)
                this.Dequeue();
            base.Enqueue(item);
        }
    }
    public class GameModel
    {
        private Map _field = null!;
        private Int32 _timeElapsed;
        private Int32 _tickCount;
        private Int32 _money;
        private LimitedQueue<Citizen> _tmpCitizens;

        public PlaySpeed GamePace { get; set; }
        public Int32 CommercialTax { get; set; }
        public Int32 IndustrialTax { get; set; }
        public Int32 ResidentialTax { get; set; }
        public Map Field => _field;

        public event EventHandler<SimCityArgsTime>? GameAdvanced;
        public event EventHandler<SimCityArgsClick>? GameBuild;

        public GameModel()
        {
            _timeElapsed = 0;
            _money = 100000;
            GamePace = PlaySpeed.Normal;
            CommercialTax = 10;
            IndustrialTax = 10;
            ResidentialTax = 10;
            _tmpCitizens = new LimitedQueue<Citizen>(100);
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
                
                if (_field.NumberOfCitizens < _field.MaxCitizens)
                {
                    var residentialZones = _field.AvailableZones("Residential").Where(y =>
                        _field[y.Item2.Item1, y.Item2.Item2].residents.Count
                        < (Int32)_field[y.Item2.Item1, y.Item2.Item2].SizeOfZone).ToList();
                    
                    Citizen toPlace = new Citizen(Enumerable
                        .Range(1, Int32.MaxValue)
                        .First(y => !Field.GetCitizens().Select(x => x.CitizenId)
                            .Concat(_tmpCitizens.Select(c => c.CitizenId)).Contains(y)));
                    
                    if (residentialZones.Count != 0)
                    {
                        Int32 minHappiness = residentialZones.Min(y => y.Item1.Happiness) - 1;
                        List<(Int32, Int32)> happinessBasedFieldCounts = residentialZones
                            .Select(y => (y.Item1.Happiness - minHappiness, y.Item2))
                            .SelectMany(y => Enumerable.Repeat(y.Item2, y.Item1)).ToList();

                        Random randSelector = new Random();
                        (Int32, Int32) selectedField =
                            happinessBasedFieldCounts.ElementAt(randSelector.Next(happinessBasedFieldCounts.Count));
                        
                        if(_tmpCitizens.Count == 0)
                            _field[selectedField.Item1, selectedField.Item2].residents.Add(toPlace);
                        else
                        {
                            _field[selectedField.Item1, selectedField.Item2].residents.Add(_tmpCitizens.Dequeue());
                            _tmpCitizens.Enqueue(toPlace);
                        }
                    }
                    else 
                        _tmpCitizens.Enqueue(toPlace);
                }

                Object MoneyLock = new object();
                Task.Run(() =>
                {
                    Int32 tax = Field.TickZones(CommercialTax, IndustrialTax, ResidentialTax).Result;
                    lock (MoneyLock)
                    {
                        _money += tax;
                    }
                });
                Task.Run(() =>
                {
                    var inhabitedResidentialZones = _field.AvailableZones("Residential")
                        .Select(y => y.Item1).Where(y => y.IsInhabited);

                    var residentsToMoveOut = inhabitedResidentialZones.SelectMany(y => y.residents)
                        .ToList();
                    
                    foreach (Citizen resident in residentsToMoveOut)
                    {
                        resident.MoveOut();
                        _tmpCitizens.Enqueue(resident);
                    }

                });

                this.GameAdvanced?.Invoke(this, new SimCityArgsTime(_timeElapsed, _field.NumberOfCitizens, _money));
            }

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
