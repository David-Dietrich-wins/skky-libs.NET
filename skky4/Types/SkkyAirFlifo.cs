using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Types
{
    public class SkkyAirFlifo
    {
        public string EquipmentType { get; set; }

		public string Duration { get; set; }

        public int NumberOfStops { get; set; }

		public void Clear()
        {
            EquipmentType = string.Empty;
            Duration = string.Empty;
            NumberOfStops = 0;
        }
    }
}
