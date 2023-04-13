using System;

namespace SimCity.Persistence;

public enum AreaType { None, Rode, Living, Commercial, Industrial}

/// <summary>
/// Class <c>Map</c> storing the necessary map data 
/// </summary>
public class Map
{
    private AreaType[,] _fields;

    public AreaType this[Int32 x, Int32 y] => _fields[x, y];
    
    /// <summary>
    /// Constructor <c>Map</c> creating the desired map size and setting up the basic values
    /// </summary>
    public Map(Int32 rows, Int32 columns)
    {
        _fields = new AreaType[rows, columns];

        for (var i = 0; i < rows * columns; ++i)
            _fields[i / rows, i % columns] = AreaType.None;
    }

    public void Build(Int32 row, Int32 column, AreaType toBuild)
    {
        
    }

    public void Remove(Int32 row, Int32 column)
    {
        
    }
}