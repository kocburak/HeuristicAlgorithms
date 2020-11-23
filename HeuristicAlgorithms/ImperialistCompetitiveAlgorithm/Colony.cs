using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicAlgorithms.ICA
{
    public class Colony : ICloneable
    {
        public double[] Position;
        public double Fittness;

        public object Clone()
        {
            return new Colony
            {
                Fittness = this.Fittness,
                Position = this.Position.Select(p => p).ToArray()
            };
        }
    }
}
