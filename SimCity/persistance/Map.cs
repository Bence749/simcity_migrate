using System;

namespace SimCity.Persistence;

public enum AreaType { None, Rode, Living, Commercial, Industrial}

/// <summary>
/// Class <c>Map</c> storing the necessary map data 
/// </summary>
public class Map
{
    private AreaType[,] _fields;

    private AreaType this[Int32 x, Int32 y] => _fields[x, y];
    
    /// <summary>
    /// Constructor <c>Map</c> creating the desired map size and setting up the basic values
    /// </summary>
    public Map()
    {
        _fields = new AreaType[256, 256];

        for (var i = 0; i < 256 * 256; ++i)
            _fields[i / 256, i % 256] = AreaType.None;
    }
}