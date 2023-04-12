using System;
using System.Collections.Generic;

namespace SimCity.Model
{
    public class SimCityArgs
    {
        public readonly Int32 TimeElapsed;

        public SimCityArgs(Int32 timeElapsed)
        {
            this.TimeElapsed = timeElapsed;
        }
    }
}