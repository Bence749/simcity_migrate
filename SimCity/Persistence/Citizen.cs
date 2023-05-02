using System;

namespace SimCity.Persistence;

public class Citizen
{
    public Int32 Income { get; set; }= 0;
    public Int32? WorkplaceID { get; set; } = null;
    
    public Citizen() { }
}