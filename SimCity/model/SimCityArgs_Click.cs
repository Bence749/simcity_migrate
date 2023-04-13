using System;
using System.Collections.Generic;

namespace SimCity.Model
{
    public class SimCityArgsClick
    {
        public readonly Int32 Money;

        public SimCityArgsClick(Int32 timeElapsed, Int32 citizens, Int32 money)
        {
            this.Money = money;
        }
    }
}