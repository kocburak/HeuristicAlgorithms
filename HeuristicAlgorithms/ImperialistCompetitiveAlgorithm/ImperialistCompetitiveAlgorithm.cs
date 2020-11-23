using HeuristicAlgorithms.GSA;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeuristicAlgorithms.ICA
{
    public class ImperialistCompetitiveAlgorithm
    {
        public IList<Iteration<Colony>> Iterations;
        public int NumColony;
        public int MaxIteration;
        public int NumDimensions;
        public double MinSearchValue;
        public double MaxSearchValue;
        public IFitFunction Function;

        private readonly Random rand = new Random();

        public void FindSolution()
        {
            var firstIteration = new Iteration<Colony>
            {
                Agents = GenerateRandomColony()
            };
            Iterations = new List<Iteration<Colony>>
            {
                firstIteration
            };
            var currentIteration = firstIteration;

            while (Iterations.Count <= MaxIteration)
            {
                //TODO: Yaz burayı
            }
        }

        private List<Colony> GenerateRandomColony()
        {
            List<Colony> colonies = new List<Colony>();
            for (int i = 0; i < NumColony; i++)
            {
                var colony = new Colony
                {
                    Position = new double[NumDimensions]
                };

                for (int j = 0; j < NumDimensions; j++)
                {
                    colony.Position[j] = MinSearchValue + (MaxSearchValue - MinSearchValue) * rand.NextDouble();
                }

                colonies.Add(colony);
            }
            return colonies;
        }
    }
}
