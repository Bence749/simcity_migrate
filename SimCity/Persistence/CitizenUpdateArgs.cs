using System;

namespace SimCity.Persistence;

public class CitizenUpdateArgs
{
    public readonly Int32 areaId;
    public readonly Int32 salary;
    
    public CitizenUpdateArgs(Int32 areaId, Int32 salary)
    {
        this.areaId = areaId;
        this.salary = salary;
    }
}