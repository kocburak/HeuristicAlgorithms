using System;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicAlgorithms.GSA
{
    public class Iteration<T> : ICloneable where T : ICloneable
    {
        public List<T> Agents;

        public object Clone()
        {
            return new Iteration<T>
            {
                Agents = this.Agents.Select(a => (T)a.Clone()).ToList()
            };
        }
    }
}
