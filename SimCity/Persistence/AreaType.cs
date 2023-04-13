using System;

namespace SimCity.Persistence;

public class AreaType
    {
        public Int32 MaintenanceCost { get; }

        public Int32 BuildCost { get; }

        public Int32 RemovePrice { get; }

        public AreaType(Int32 maintenanceCost = 0, Int32 buildCost = 0, Int32 removePrice = 0)
        {
            this.MaintenanceCost = maintenanceCost;
            this.BuildCost = buildCost;
            this.RemovePrice = removePrice;
        }

        public virtual String GetAreaType() => "None";
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