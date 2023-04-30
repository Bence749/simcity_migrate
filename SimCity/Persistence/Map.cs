using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace SimCity.Persistence;

/// <summary>
/// Class <c>Map</c> storing the necessary map data 
/// </summary>
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
}