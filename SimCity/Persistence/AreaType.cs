using System;
using System.Windows.Controls.Ribbon.Primitives;

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
        public Int32 MaintenanceCost { get; }
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

        /// <summary>
        /// Initialize variables.
        /// </summary>
        /// <param name="maintenanceCost">Cost to maintain zone per tick</param>
        /// <param name="buildCost">Cost to build the zone</param>
        /// <param name="removePrice">Money you will get if you remove the zone</param>
        public AreaType(Int32 maintenanceCost = 0, Int32 buildCost = 0, Int32 removePrice = 0)
        {
            this.MaintenanceCost = maintenanceCost;
            this.BuildCost = buildCost;
            this.RemovePrice = removePrice;
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
        public virtual Int32 CalculateTax() => 0;
    }

public class Road : AreaType
{
    public Road() : base(5, 100) { }

    public override String GetAreaType() => "Road";
}

public class CommercialZone : AreaType
{
    public CommercialZone() : base(20) { }

    public override String GetAreaType() => "Commercial";
}

public class IndustrialZone : AreaType
{
    public IndustrialZone() : base(50) { }
    
    public override String GetAreaType() => "Industrial";
}

public class ResidentialZone : AreaType
{
    public ResidentialZone() : base(25) { }
    
    public override String GetAreaType() => "Residential";
}

public class Police : AreaType
{
    public Police() : base(100, 500, 100) { }
    
    public override String GetAreaType() => "Police";
}

public class Stadium : AreaType
{
    public Stadium() : base(500, 1000, 100) { }
    
    public override String GetAreaType() => "Stadium";
}

public class Tree : AreaType
{
    public Tree() : base(1, 50, 10) { }
    
    public override String GetAreaType() => "Tree";
}

public class FireDepartment : AreaType
{
    public FireDepartment() : base(200, 500, 100) { }

    public override String GetAreaType() => "FireDepartment";
}