using System;
using System.Runtime.CompilerServices;

namespace SimCity.Persistence;

public class Citizen
{
    private Int32 _citizenID;
    
    public Int32 Income { get; set; } = 0;
    public Int32? WorkplaceID { get; set; } = null;
    public Int32 CitizenID => _citizenID;
    public Int32 Happiness { get; set; } = 100;

    public Citizen(Int32 citizenId)
    {
        this._citizenID = citizenId;
    }

    public void MoveOut()
    {
        Income = 0;
        WorkplaceID = null;
        Happiness = 50;
    }
}