using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Transactions;
using System.Windows.Controls.Ribbon.Primitives;
// ReSharper disable All

namespace SimCity.Persistence;

/// <summary>
/// Define the size of the zone that set the maximum residents/employees. 
/// </summary>
public enum SizeType
{
    /// <summary> Small size = 30 residents/employees </summary>
    Small = 30,
    /// <summary> Small size = 100 residents/employees </summary>
    Medium = 100,
    /// <summary> Small size = 500 residents/employees </summary>
    Big = 500
}

/// <summary>
/// Parent AreaType that defines the basic params and function of children.
/// </summary>
public class AreaType
{
    public List<Citizen> residents = new List<Citizen>();
    
    public Int32 AreaID { get; set; } = 0;
    /// <summary>
    /// Each tick this amount will get deducted from the money.
    /// </summary>
    public Int32 MaintenanceCost { get; protected set; }
    /// <summary>
    /// Cost to build the zone
    /// </summary>
    public Int32 BuildCost { get; }
    /// <summary>
    /// Amount of money the city will get if the user demolish a zone
    /// </summary>
    public Int32 RemovePrice { get; }
    public SizeType SizeOfZone { get; set; } = SizeType.Small;
    public Int32 NumberOfWorkers { get; set; } = 0;
    public Int32 Salary { get; protected set; }
    public Int32 Happiness { get; set; } = 0;
    public Int32 AreaSize { get; protected set; }
    public Int32 HappinessInc { get; protected set; }
    public Boolean IsSpecial { get; protected set; } = false;
    public Boolean IsInhabited { get; protected set; } = false;
    protected Int32 Patience { get; set; } = 100;
    
    public event EventHandler<CitizenUpdateArgs>? UpdateEvent;
    public event EventHandler<(Int32, Int32)>? FireEvent; 

    /// <summary>
    /// Initialize variables.
    /// </summary>
    /// <param name="maintenanceCost">Cost to maintain zone per tick</param>
    /// <param name="buildCost">Cost to build the zone</param>
    /// <param name="removePrice">Money you will get if you remove the zone</param>
    /// <param name="areaSize">Describes the size of area where it increases happiness</param>
    /// <param name="happinessInc">Defines the happiness increase/decrease in an area</param>
    /// <param name="isSpecial">Specifies whether the field is special</param>
    public AreaType(Int32 maintenanceCost = 0, Int32 buildCost = 0, Int32 removePrice = 0,
        Int32 areaSize = 0, Int32 happinessInc = 0, Boolean isSpecial = false, Int32 salary = 0)
    {
        this.MaintenanceCost = maintenanceCost;
        this.BuildCost = buildCost;
        this.RemovePrice = removePrice;
        this.AreaSize = areaSize;
        this.HappinessInc = happinessInc;
        this.IsSpecial = isSpecial;
        this.Salary = salary;
    }

    protected void OnAreaUpdate(CitizenUpdateArgs e) => UpdateEvent?.Invoke(this, e);
    protected void OnFire((Int32, Int32) e) => FireEvent?.Invoke(this, e);

    /// <summary>
    /// Get the type of the area.
    /// </summary>
    /// <returns>Type of the area in String</returns>
    public virtual String GetAreaType() => "None";
    /// <summary>
    /// Calculate the tax that the city will get each tick.
    /// </summary>
    /// <returns>Int32 containing the amount of tax.</returns>
    public virtual Int32 CalculateTax(List<AreaType> neighbourAreas, Int32 taxPercent) => 0;
    public virtual Int32 CalculateTax(ref List<Citizen> citizens, Int32 taxPercent) => 0;

    public virtual Citizen? Hire(List<AreaType> neighbourAreas) => null;
    public virtual void Fire(Int32 areaID, Int32 numberOfFired) { }
    protected virtual void Upgrade() { }
    protected virtual void Upgrade(List<Citizen> citizens) { }
    protected virtual void Unhabit() { }
}

public class Road : AreaType
{
    public Road() : base(5, 100) { }

    public override String GetAreaType() => "Road";
}

public class CommercialZone : AreaType
{
    
    public CommercialZone() : base(20, areaSize: 4, salary: 200) { }

    public override String GetAreaType() => "Commercial";
    
    
    private Int32 _maxCustomers = 50;
    private Int32 _baseMaintenanceCost = 50;
    private Double RoI = 1;

    public override Int32 CalculateTax(List<AreaType> neighbourAreas, Int32 taxPercent)
    {
        if (IsInhabited) return 0;
        
        List<(Int32, Int32)> customerResidents = neighbourAreas.Where(y => y.GetAreaType() == "Residential")
            .Select(y => (y.Happiness, y.residents.Count)).ToList();
        
        //Spending
        Int32 spending = NumberOfWorkers * Salary;
        MaintenanceCost = (Int32) (1 + Happiness * 2) * _baseMaintenanceCost;
        
        //Income
        Int32 income = customerResidents.Sum(y => 
            (Int32) Math.Round((y.Item1 * 0.3 + 1) * (y.Item2 < _maxCustomers ? y.Item2 : _maxCustomers) * 50));

        Int32 tax = (Int32)Math.Round((income - spending) * (taxPercent / 100.0));
        RoI = (income - (spending + MaintenanceCost)) / (spending + MaintenanceCost);

        if (RoI < 0.3 || RoI > 0.8)
            Patience -= 1;
        else
            Patience = 100;

        if (RoI < 0.3 && Patience % (Double)Math.Round(100.0 / (Int32)SizeOfZone) == 0)
            Fire(AreaID, 100 / (Int32) SizeOfZone > 1 ? 1 : (Int32) Math.Round(1 / (100.0 / (Int32) SizeOfZone)));
        
        if(Patience == 0)
            if(RoI > 0.8)
                Upgrade();
            else
                Unhabit();
            
        return tax;
    }

    public override void Fire(Int32 areaID, Int32 numberOfFired) => OnFire((areaID, numberOfFired));

    public override Citizen? Hire(List<AreaType> neighbourAreas)
    {
        if (RoI < 0.3) return null;
        
        Random rnd = new Random();
        var unemployed = neighbourAreas.Where(y => y.GetAreaType() == "Residential")
            .SelectMany(y => y.residents).Where(y => y.WorkplaceId == null && y.CitizenId > 0).OrderBy(y => rnd.Next())
            .ToList().FirstOrDefault();

        return unemployed;
    }

    protected override void Upgrade()
    {
        if (SizeOfZone == SizeType.Big) return;
        
        _baseMaintenanceCost *= 5;
        _maxCustomers *= 3;
        AreaSize += 3;
        Salary = (Int32)Math.Round(Salary * 1.5);
        
        SizeOfZone = SizeOfZone == SizeType.Small ? SizeType.Medium : SizeType.Big;
        
        OnAreaUpdate(new CitizenUpdateArgs(AreaID, Salary));
    }

    protected override void Unhabit()
    {
        this.IsInhabited = true;
        
        OnAreaUpdate(new CitizenUpdateArgs(AreaID, 0));
    }
}

public class IndustrialZone : AreaType
{
    public IndustrialZone() : base(50, areaSize:10, happinessInc:-20) { }
    
    public override String GetAreaType() => "Industrial";
}

public class ResidentialZone : AreaType
{
    private Int32 _costOfLiving;

    public ResidentialZone() : base(25)
    {
        _costOfLiving = 150;
    }

    public override int CalculateTax(ref List<Citizen> citizens, int taxPercent)
    {
        Int32 totalIncome = citizens.Select(y => y.Income).Where(y => y != 0).Sum();
        Int32 tax = (Int32) Math.Round(totalIncome * (taxPercent / 100.0));

        foreach (var citizen in citizens)
        {
            if (_costOfLiving / citizen.Income > 0.8)
                citizen.Happiness--;
            else if (_costOfLiving / citizen.Income < 0.5)
                citizen.Happiness++;
        }

        double citizensHappiness = citizens.Select(y => y.Happiness).Average();
        if(citizensHappiness < 0.2 || citizensHappiness > 0.8)
            Patience -= 1;
        else
            Patience = 100;
        
        if(Patience == 0)
            if(citizensHappiness > 0.8)
                Upgrade();
            else
                Unhabit();
        
        
        
        return tax;
    }

    protected override void Upgrade()
    {
        if (SizeOfZone == SizeType.Big) return;

        _costOfLiving *= 3;
        SizeOfZone = SizeOfZone == SizeType.Small ? SizeType.Medium : SizeType.Big;
    }

    protected override void Unhabit()
    {
        this.IsInhabited = true;
    }

    public override String GetAreaType() => "Residential";
}

public class Police : AreaType
{
    public Police() : base(100, 500, 100, 15, 10, true) { }
    
    public override String GetAreaType() => "Police";
}
public class FireDepartment : AreaType
{
    public FireDepartment() : base(200, 500, 100, 15, 10, true) { }

    public override String GetAreaType() => "FireDepartment";
}

public class Stadium : AreaType
{
    public Stadium() : base(500, 1000, 100, 10, 8, true) { }
    
    public override String GetAreaType() => "Stadium";
}

public class Tree : AreaType
{
    public Tree() : base(1, 50, 10, 7, 3, true) { }
    
    public override String GetAreaType() => "Tree";
}

