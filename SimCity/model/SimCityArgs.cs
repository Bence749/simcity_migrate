using System;
using System.Collections.Generic;

namespace SimCity.Model
{
    public class SimCityArgs
    {
        public readonly Int32 TimeElapsed;
        public readonly Int32 Citizens;
        public readonly Int32 Money;

        public SimCityArgs(Int32 timeElapsed, Int32 citizens, Int32 money)
        {
            this.TimeElapsed = timeElapsed;
            this.Citizens = citizens;
            this.Money = money;
        }
    }
}