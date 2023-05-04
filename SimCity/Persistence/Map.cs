using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace SimCity.Persistence;

/// <summary>
/// Class <c>Map</c> storing the necessary map data 
/// </summary>
[SuppressMessage("ReSharper", "PossibleLossOfFraction")]
public class Map
{
    private AreaType[,] _fields;

    public AreaType this[Int32 x, Int32 y] => _fields[x, y];
    public Int32 RowSize => _fields.GetLength(0);
    public Int32 ColumnSize => _fields.GetLength(1);
    
    public Int32 NumberOfCitizens => _fields.Cast<AreaType>().Select(y => y.residents.Count).Sum();

    public Int32 MaxCitizens => _fields.Cast<AreaType>().Select(y => (Int32) y.SizeOfZone).Sum();

    /// <summary>
    /// Constructor <c>Map</c> creating the desired map size and setting up the basic values
    /// </summary>
    public Map(Int32 rows, Int32 columns)
    {
        _fields = new AreaType[rows, columns];

        for (var i = 0; i < rows * columns; ++i)
            _fields[i / rows, i % columns] = new AreaType();
        
        if(rows % 2 == 1)
        {
            double middle = rows / 2;
            _fields[(int) Math.Floor(middle) + 1, 0] = _fields[(int) Math.Ceiling(middle) - 1, 0] = new Road();
        }
        else
            _fields[rows / 2 - 1, 0] = _fields[(rows / 2) + 1, 0] = new Road();
    }
    
    public Int32 GetMaintenance()
    {
        Int32 sumMaintenance = 0;

        foreach (AreaType field in _fields)
        {
            sumMaintenance += field.MaintenanceCost;
        }
        
        return sumMaintenance;
    }
    
    /// <summary>
    /// Set the <c>_fields</c> value at position <c>row, column</c> to the requested child of <see cref="AreaType" />
    /// </summary>
    /// <param name="row">Row of the desired element.</param>
    /// <param name="column">Column of the desired element</param>
    /// <param name="toBuild">Child of <see cref="AreaType" /> to be built.</param>
    /// <returns><see cref="Int32" /> cost to be deducted from the players balance.</returns>
    /// <exception cref="PersistenceExceptions">Throws this when <c>Build</c> is not possible.</exception>
    public Int32 Build(Int32 row, Int32 column, AreaType toBuild)
    {
        if (_fields[row, column].GetAreaType() != "None" || toBuild.GetAreaType() == "None") 
            throw new PersistenceExceptions("Selected area is not empty or the input is the parent class!");
        if (NeighbouringFields(row, column).All(f => f.Item1.GetAreaType() != "Road"))
            throw new PersistenceExceptions("Selected area is not surrounded by a road.");

        toBuild.Happiness = _fields[row, column].Happiness;
        if (toBuild.HappinessInc != 0)
        {
            var neighbourFields = NeighbouringFields(row, column, toBuild.AreaSize);
            foreach (var fields in neighbourFields.Select(y => y.Item2))
                _fields[fields.Item1, fields.Item2].Happiness += toBuild.HappinessInc;
        }

        toBuild.AreaID = Enumerable.Range(1, Int32.MaxValue)
            .First(y => !_fields.Cast<AreaType>().Select(y => y.AreaID).Contains(y));
        _fields[row, column] = toBuild;
        if (_fields[row, column].GetType().GetMethod("Upgrade") != null)
        {
            _fields[row, column].UpdateEvent += UpdateCitizens!;
            _fields[row, column].FireEvent += FireCitizens!;
        }

        return toBuild.BuildCost;
    }

    /// <summary>
    /// Set the <c>_fields</c> value at position <c>row, column</c> to <see cref="AreaType" /> (Parent class)
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns>Money amount to be added to the players balance</returns>
    /// <exception cref="PersistenceExceptions">Throws this when <c>Remove</c> is not possible.</exception>
    public Int32 Remove(Int32 row, Int32 column)
    {
        if (_fields[row, column].GetAreaType() == "None") 
            throw new PersistenceExceptions("Selected area is already empty!");
        Int32 prize = _fields[row, column].RemovePrice;
        AreaType newField = new AreaType
        {
            Happiness = _fields[row, column].Happiness
        };
        if (_fields[row, column].IsSpecial)
        {
            var neighbourFields = NeighbouringFields(row, column, _fields[row, column].AreaSize);
            foreach (var fields in neighbourFields.Select(y => y.Item2))
                _fields[fields.Item1, fields.Item2].Happiness -= _fields[row, column].HappinessInc;
        }
        _fields[row, column] = newField;
        return prize;
    }

    /// <summary>
    /// Get all zone with the given <see cref="AreaType" />
    /// </summary>
    /// <param name="areaType">Type of the area to search for.</param>
    /// <returns>A list containing x and y coordinates where the given Area occurs</returns>
    public List<(AreaType, (Int32, Int32))> AvailableZones(String areaType)
    {
        List<(AreaType, (Int32, Int32))> output = _fields.Cast<AreaType>()
            .Select((value, index) => 
                new { Value = value, 
                    Index = (index / _fields.GetLength(0), index % _fields.GetLength(1))})
            .Where(y => y.Value.GetAreaType() == areaType).Select(y => (y.Value, y.Index)).ToList();
        
        return output;
    }

    public async Task<Int32> TickZones(Int32 commercialTax, Int32 industrialTax, Int32 resindentialTax)
    {
        var zonesToTick = _fields.Cast<AreaType>()
            .Select((value, index) =>
                new
                {
                    Value = value,
                    Index = (index / _fields.GetLength(0), index % _fields.GetLength(1))
                })
            .Where(y => y.Value.GetType().GetMethod("CalculateTax") != y.Value.GetType()).ToList();

        Int32 taxIncomeSum = 0;
        Object taxIncomeSumLock = new Object();
        Object citizensLock = new Object();
        foreach (var zone in zonesToTick)
        {
            await Task.Run(() =>
            {
                Int32 taxIncome;
                if (zone.Value.GetAreaType() != "Resindential")
                {
                    taxIncome = zone.Value
                        .CalculateTax(NeighbouringFields(zone.Index.Item1, zone.Index.Item2, zone.Value.AreaSize)
                                .Select(y => y.Item1).ToList(),
                            zone.Value.GetAreaType() == "Commercial" ? commercialTax : industrialTax);
                }
                else
                    taxIncome = zone.Value.CalculateTax(ref _fields[zone.Index.Item1, zone.Index.Item2].residents,
                        resindentialTax);
                lock (taxIncomeSumLock)
                {
                    taxIncomeSum += taxIncome;
                }
            });
            if ((zone.Value.GetType().GetMethod("Hire") != zone.Value.GetType())
                && zone.Value.NumberOfWorkers < (Int32)zone.Value.SizeOfZone)
                await Task.Run(() =>
                {
                    Citizen? toHire = _fields[zone.Index.Item1, zone.Index.Item2]
                        .Hire(NeighbouringFields(zone.Index.Item1, zone.Index.Item2, 
                            ((Int32) zone.Value.AreaSize) * 2).Select(y => y.Item1).ToList());
                    
                    if(toHire is not null)
                        lock (citizensLock)
                        {
                            var citizenToHireLoc = _fields.Cast<AreaType>()
                                .Select((y, index) => new
                                {
                                    Value = y, 
                                    Index = (index / _fields.GetLength(0), index % _fields.GetLength(1))
                                }).Where(y => y.Value.GetAreaType() == "Residential")
                                .First(y => y.Value.residents.Contains(toHire)).Index;

                            Int32 citizenIndex = _fields[citizenToHireLoc.Item1, citizenToHireLoc.Item2].residents
                                .FindIndex(y => y == toHire);
                            _fields[citizenToHireLoc.Item1, citizenToHireLoc.Item2].residents[citizenIndex].WorkplaceId =
                                zone.Value.AreaID;
                            _fields[citizenToHireLoc.Item1, citizenToHireLoc.Item2].residents[citizenIndex].Income =
                                zone.Value.Salary;
                        }
                });
        }

        return taxIncomeSum;
    }

    /// <summary>
    /// Returns a list of neighbouring fields within the specified area around the given row and column indices.
    /// </summary>
    /// <param name="row">Row of the center field</param>
    /// <param name="col">Column of the center field</param>
    /// <param name="area">[Optional] Size of area. Default value is 1.</param>
    /// <returns>All fields <see cref="AreaType" /> and according indices around element at <c>row</c> and <c>col</c> in an <c>area</c></returns>
    /// <remarks>
    /// This method searches for neighbouring fields within a square area of size (2 * area + 1) x (2 * area + 1) centered around the specified row and column indices.
    /// The method returns a list of tuples, where each tuple contains the field value and its row and column indices.
    /// </remarks>
    public List<(AreaType, (Int32, Int32))> NeighbouringFields(Int32 row, Int32 col, Int32 area = 1)
    {
        var output = (from i in Enumerable.Range(row - area, 1 + 2 * area)
                                       from j in Enumerable.Range(col - area, 1 + 2 * area)
                                       where i >=  0 && i < RowSize && j >= 0 
                                             && j < ColumnSize && (i != row || j != col)
                                       select (_fields[i, j], (i, j))).ToList();

        return output;

    }

    public List<Citizen> GetCitizens()
    {
        List<Citizen> citizens = _fields.Cast<AreaType>()
            .Where(y => y.GetAreaType() == "Residential")
            .SelectMany(y => y.residents).ToList();

        return citizens;
    }

    private void UpdateCitizens(object sender, CitizenUpdateArgs e)
    {
        for(Int32 i = 0; i < _fields.GetLength(0); ++i)
            for (Int32 j = 0; j < _fields.GetLength(1); ++j)
            {
                if (_fields[i, j].GetAreaType() == "Residential" &&
                    _fields[i, j].residents.Select(y => y.WorkplaceId).Contains(e.areaId))
                {
                    List<Int32> indices = _fields[i, j].residents
                        .Select((value, index) => new { Value = value.WorkplaceId, Index = index })
                        .Where(y => y.Value == e.areaId).Select(y => y.Index).ToList();

                    foreach (Int32 index in indices)
                        _fields[i, j].residents[index].Income = e.salary;
                }
            }
    }

    private void FireCitizens(object sender, (Int32, Int32) e)
    {
        for (Int32 i = 0; i < e.Item2; ++i)
        {
            Random rnd = new Random();
            var toFireResidentIndex = _fields.Cast<AreaType>()
                .Where(y => y.GetAreaType() == "Residential")
                .Select((value, index) =>
                    new
                    {
                        Value = value,
                        Index = (index / _fields.GetLength(0), index % _fields.GetLength(1))
                    }).Where(y => y.Value.residents.Select(x => x.WorkplaceId).Contains(e.Item1))
                .OrderBy(y => rnd.Next()).First();

            Int32 residentToFireIndex = _fields[toFireResidentIndex.Index.Item1, toFireResidentIndex.Index.Item2]
                .residents.FindIndex(y => y.WorkplaceId == e.Item1);

            _fields[toFireResidentIndex.Index.Item1, toFireResidentIndex.Index.Item2].residents[residentToFireIndex]
                .MoveOut();
        }
    }
}