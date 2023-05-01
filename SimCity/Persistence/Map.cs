using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media;

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

    public Int32 maxCitizens()
    {
        Int32 sumCit = 0;

        foreach (var field in _fields)
            if (field.GetAreaType() == "Residential")
                sumCit += (int) field.SizeOfZone;

        return sumCit;
    }
    
    public Int32 getMaintenance()
    {
        Int32 sumMaintenance = 0;

        foreach (AreaType field in _fields)
        {
            sumMaintenance += field.MaintenanceCost;
        }
        
        return sumMaintenance;
    }
    
    public Int32 Build(Int32 row, Int32 column, AreaType toBuild)
    {
        if (_fields[row, column].GetAreaType() != "None") return 0;
        if (NeighbouringFields(row, column).All(f => f.GetAreaType() != "Road")) return 0;
        _fields[row, column] = toBuild;
        return toBuild.BuildCost;
    }

    public Int32 Remove(Int32 row, Int32 column)
    {
        if (_fields[row, column].GetAreaType() == "None") return 0;
        Int32 prize = _fields[row, column].RemovePrice;
        _fields[row, column] = new AreaType();
        return prize;
    }

    /// <summary>
    /// Get all zone with the given AreaType
    /// </summary>
    /// <param name="areaType">Type of the area to search for.</param>
    /// <returns>A list containing x and y coordinates where the given Area occurs</returns>
    public List<(Int32, Int32)> AvaibleZones(String areaType)
    {
        List<(Int32, Int32)> output = new List<(int, int)>();

        var indexedFields = _fields.Cast<AreaType>()
            .Select((value, index) => 
                new { Index = (index / _fields.GetLength(0), index % _fields.GetLength(1)),
                    Value = value });
        foreach (var area in indexedFields)
            if (area.Value.GetAreaType() == areaType)
                    output.Add(area.Index);

        return output;
    }

    private List<AreaType> NeighbouringFields(Int32 row, Int32 col)
    {
        var output = from i in Enumerable.Range(row - 1, 3)
                                       from j in Enumerable.Range(col - 1, 3)
                                       where i >=  0 && i < RowSize && j >= 0 
                                             && j < ColumnSize && (i != row || j != col)
                                       select _fields[i, j];

        return output.ToList();

    }
}