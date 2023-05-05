using System;
using System.Runtime.CompilerServices;

namespace SimCity.Persistence;

public class Citizen
{
    public Int32 Income { get; set; } = 0;
    public Int32? WorkplaceId { get; set; } = null;
    public Int32 CitizenId { get; }

    public Int32 Happiness { get; set; } = 100;

    public Citizen(Int32 citizenId)
    {
        this.CitizenId = citizenId;
    }

    public void MoveOut()
    {
        Income = 0;
        WorkplaceId = null;
        Happiness = 50;
    }
}