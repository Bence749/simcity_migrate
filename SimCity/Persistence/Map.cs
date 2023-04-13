using System;

namespace SimCity.Persistence;

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
            _fields[i / rows, i % columns] = new AreaType();
    }

    public Int32 getMaintenance()
    {
        Int32 sumMaintenance = 0;

        foreach (AreaType field in _fields)
        {
            switch (field.GetAreaType())
            {
                case "Road":
                    
                case "Residential": sumMaintenance += field.MaintenanceCost;
                    break;
                case "Commercial": sumMaintenance += field.MaintenanceCost;
                    break;
                case "Industrial": sumMaintenance += field.MaintenanceCost;
                    break;
            }
        }
        
        return sumMaintenance;
    }
    
    public Int32 Build(Int32 row, Int32 column, AreaType toBuild)
    {
        switch (toBuild.GetAreaType())
        {
            case "Road":
                if (_fields[row, column].GetAreaType() == "None")
                {
                    _fields[row, column] = new Road();
                    return toBuild.BuildCost;
                }

                break;
        }

        return 0;
    }

    public void Remove(Int32 row, Int32 column)
    {
        
    }
}