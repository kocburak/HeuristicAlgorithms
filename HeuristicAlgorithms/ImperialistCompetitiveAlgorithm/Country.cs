using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicAlgorithms.ICA
{
    public class Country : ICloneable
    {
        public double[] Position;
        public double Cost;
        public double NormalizedCost;
        public double NormalizedPower;

        public IList<Country> Colonies;

        public object Clone()
        {
            return new Country
            {
                Cost = this.Cost,
                NormalizedCost = this.NormalizedCost,
                NormalizedPower = this.NormalizedPower,
                Position = this.Position.Select(p => p).ToArray(),
                Colonies = this.Colonies?.Select(p => p).ToArray()
            };
        }
    }
}
