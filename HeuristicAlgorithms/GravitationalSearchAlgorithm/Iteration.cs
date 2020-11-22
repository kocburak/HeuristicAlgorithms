using System;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicAlgorithms
{
    public class Iteration : ICloneable
    {
        public List<Agent> Agents;

        public object Clone()
        {
            return new Iteration
            {
                Agents = this.Agents.Select(a => (Agent)a.Clone()).ToList()
            };
        }
    }
}
