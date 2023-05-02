using System;
using System.Runtime.CompilerServices;

namespace SimCity.Persistence;

public class Citizen
{
    private Int32 _citizenID;
    
    public Int32 Income { get; set; }= 0;
    public Int32? WorkplaceID { get; set; } = null;
    public Int32 CitizenID => _citizenID;

    public Citizen(Int32 citizenId)
    {
        this._citizenID = citizenId;
    }
}