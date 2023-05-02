using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
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
    /// <summary> Small size = 10 residents/employees </summary>
    Small = 15,
    /// <summary> Small size = 25 residents/employees </summary>
    Medium = 25,
    /// <summary> Small size = 50 residents/employees </summary>
    Big = 50
}

/// <summary>
/// Parent AreaType that defines the basic params and function of children.
/// </summary>
public class AreaType
    {
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
        public Int32 NumberOfResidents { get; set; } = 0;
        public Int32 Happiness { get; set; } = 0;
        public Int32 AreaSize { get; protected set; }
        public Int32 HappinessInc { get; protected set; }
        public Boolean IsSpecial { get; protected set; } = false;
        public Boolean IsUnhabited { get; protected set; } = false;
        protected Int32 Patience { get; set; } = 100;

        /// <summary>
        /// Initialize variables.
        /// </summary>
        /// <param name="maintenanceCost">Cost to maintain zone per tick</param>
        /// <param name="buildCost">Cost to build the zone</param>
        /// <param name="removePrice">Money you will get if you remove the zone</param>
        /// <param name="areaSize">Describes the size of area where it increases happiness</param>
        /// <param name="isSpecial">Specifies whether the field is special</param>
        public AreaType(Int32 maintenanceCost = 0, Int32 buildCost = 0, Int32 removePrice = 0,
            Int32 areaSize = 0, Int32 happinessInc = 0, Boolean isSpecial = false)
        {
            this.MaintenanceCost = maintenanceCost;
            this.BuildCost = buildCost;
            this.RemovePrice = removePrice;
            this.AreaSize = areaSize;
            this.HappinessInc = happinessInc;
            this.IsSpecial = isSpecial;
        }
        /// <summary>
        /// Get the type of the area.
        /// </summary>
        /// <returns>Type of the area in String</returns>
        public virtual String GetAreaType() => "None";
        /// <summary>
        /// Calculate the tax that the city will get each tick.
        /// </summary>
        /// <returns>Int32 containing the amount of tax.</returns>
        public virtual Int32 CalculateTax(List<AreaType> neighbouringAreas, Int32 taxPercent) => 0;

        protected virtual void Upgrade() { }
        protected virtual void Unhabit() { }
    }

public class Road : AreaType
{
    public Road() : base(5, 100) { }

    public override String GetAreaType() => "Road";
}

public class CommercialZone : AreaType
{
    
    public CommercialZone() : base(20, areaSize: 4) { }

    public override String GetAreaType() => "Commercial";

    private Int32 maxCustomers = 100;
    private Int32 baseMaintenanceCost = 20;
    
    public override Int32 CalculateTax(List<AreaType> neighbouringAreas, Int32 taxPercent)
    {
        if (IsUnhabited) return 0;
        
        List<(Int32, Int32)> customerResidents = neighbouringAreas.Where(y => y.GetAreaType() == "Residential")
            .Select(y => (y.Happiness, y.NumberOfResidents)).ToList();
        
        //Spending
        Int32 spending = NumberOfResidents * 200;
        MaintenanceCost = (Int32) (1 + Happiness * 2) * baseMaintenanceCost;
        
        //Income
        Int32 income = customerResidents.Sum(y => 
            (Int32) Math.Round((y.Item1 * 0.3 + 1) * (y.Item2 < maxCustomers ? y.Item2 : maxCustomers) * 50));

        Int32 tax = (Int32)Math.Round((income - spending) * (taxPercent / 100.0));
        Double RoI = (income - (spending + MaintenanceCost)) / (spending + MaintenanceCost);

        if (RoI < 0.2 || RoI > 0.8)
            Patience -= 1;
        else
            Patience -= 1;
        
        if(Patience == 0)
            if(RoI > 0.8)
                Upgrade();
            else
                Unhabit();
            
        return tax;
    }

    protected override void Upgrade()
    {
        if (SizeOfZone == SizeType.Big) return;
        
        baseMaintenanceCost *= 5;
        maxCustomers *= 3;
        AreaSize += 3;
        
        SizeOfZone = SizeOfZone == SizeType.Small ? SizeType.Medium : SizeType.Big;
    }

    protected override void Unhabit()
    {
        this.IsUnhabited = true;
    }
}

public class IndustrialZone : AreaType
{
    public IndustrialZone() : base(50, areaSize:10, happinessInc:-20) { }
    
    public override String GetAreaType() => "Industrial";
}

public class ResidentialZone : AreaType
{
    public ResidentialZone() : base(25) { }

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

public class Unhabited : AreaType
{
    public Unhabited() : base() { }

    public override String GetAreaType() => "Unhabited";
}

