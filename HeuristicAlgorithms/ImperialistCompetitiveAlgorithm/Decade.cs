using System;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicAlgorithms.ICA
{
    public class Decade : ICloneable
    {
        public List<Empire> Imperialists;

        public object Clone()
        {
            return new Decade
            {
                Imperialists = this.Imperialists.Select(a => (Empire)a.Clone()).ToList()
            };
        }
    }
}
