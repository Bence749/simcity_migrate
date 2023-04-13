using System;

namespace SimCity.Persistence;

public enum AreaType { None, Road, Living, Commercial, Industrial}

/// <summary>
/// Class <c>Map</c> storing the necessary map data 
/// </summary>
public class Map
{
    #region Variable vars

    private Int32 roadCost = 100;

    #endregion
    
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

    public Int32 Build(Int32 row, Int32 column, AreaType toBuild)
    {
        switch (toBuild)
        {
            case AreaType.Road:
                if (_fields[row, column] == AreaType.None)
                {
                    _fields[row, column] = AreaType.Road;
                    return roadCost;
                }

                break;
        }

        return 0;
    }

    public void Remove(Int32 row, Int32 column)
    {
        
    }
}