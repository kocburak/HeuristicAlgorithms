using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicAlgorithms.GSA;

namespace HeuristicAlgorithms.GBMO
{
    public class GasesBrownianMotionOptimization
    {
        public IList<CustomIteration> Iterations;
        public int MaxIteration;
        public int NumAgents;
        public int NumDimensions;
        public double MinSearchValue;
        public double MaxSearchValue;
        public IFitFunction Function;

        public OptimizationType OptimizationType;


        private readonly Random rand = new Random();


        private double Temperature = 1000;
        private double BoltzmanConstant = 1000;

        public GasesBrownianMotionOptimization(IFitFunction function, OptimizationType optimizationType, int numAgents, int numDimensions, int maxIteration, double minSearchValue, double maxSearchValue)
        {
            MaxIteration = maxIteration;
            NumAgents = numAgents;
            NumDimensions = numDimensions;
            MinSearchValue = minSearchValue;
            MaxSearchValue = maxSearchValue;
            Function = function;
            OptimizationType = optimizationType;
        }


        public void FindSolution()
        {
            var molecules = GenerateRandomAgents();

            CalculateFitness(molecules);

            CalculateMass(molecules);

            var firstIteration = new CustomIteration
            {
                Agents = molecules
            };

            Iterations = new List<CustomIteration>
            {
                (CustomIteration) firstIteration.Clone()
            };


            while (Iterations.Count <= MaxIteration && Temperature >= 0)
            {
                Movements(molecules);


                foreach (Molecule agent in molecules)
                {
                    for (int i = 0; i < agent.Position.Length; i++)
                    {
                        if (agent.Position[i] < MinSearchValue || agent.Position[i] > MaxSearchValue)
                        {
                            agent.Position[i] = MinSearchValue + (MaxSearchValue - MinSearchValue) * rand.NextDouble();
                        }
                    }
                }

                CalculateFitness(molecules);

                CalculateMass(molecules);

                CalculateTemperature(molecules);

                Iterations.Add(new CustomIteration() { Agents =  molecules.Select(a => (Molecule)a.Clone()).ToList(), Temperature = this.Temperature });

            }
        }

        private void CalculateTemperature(List<Molecule> molecules)
        {
            /* 
             // Bu kısım paper daki hali
            var avareageFittness = molecules.Select(m => m.Fittness).Average();

            Temperature -= (1 / avareageFittness);
            */

            var avareageFittness = molecules.Select(m => m.Fittness).Average();

            IEnumerable<Molecule> orderedMolecules = null;

            if (OptimizationType == OptimizationType.Maximization)
            {
                orderedMolecules = molecules.OrderByDescending(a => a.Fittness);
            }
            else if (OptimizationType == OptimizationType.Minimization)
            {
                orderedMolecules = molecules.OrderBy(a => a.Fittness);
            }

            var bestFittness = orderedMolecules.FirstOrDefault().Fittness;

            var worstFittness = orderedMolecules.Last().Fittness;


            var stage = (avareageFittness - worstFittness) / (bestFittness - worstFittness);

            //       Temperature -= (1 / 1 - stage);

            Temperature -= (1 / avareageFittness);

        }

        public void Movements(IEnumerable<Molecule> molecules)
        {
            foreach (Molecule agent in molecules)
            {
                for (int i = 0; i < NumDimensions; i++)
                {
                    /* 
                     // Bu kısım paper daki hali
                            agent.Velocity[i] = agent.Velocity[i] + Math.Sqrt(3 * BoltzmanConstant * Temperature / agent.Mass);
                    */


                    agent.Velocity[i] += 2*(rand.NextDouble() - 0.5) * Math.Sqrt(3 * BoltzmanConstant * Temperature / (agent.Mass +  0.0001));

                    agent.Position[i] += agent.Velocity[i];

                    agent.Position[i] += (0.2 - (0.5 / (2 * Math.PI)) * Math.Sin(2 * Math.PI * agent.Position[i])) % 1;
                }
            }
        }

        private void CalculateMass(IEnumerable<Molecule> molecules)
        {

            IEnumerable<Molecule> orderedMolecules = null;

            if (OptimizationType == OptimizationType.Maximization)
            {
                orderedMolecules = molecules.OrderByDescending(a => a.Fittness);
            }
            else if (OptimizationType == OptimizationType.Minimization)
            {
                orderedMolecules = molecules.OrderBy(a => a.Fittness);
            }

            var bestFittness = orderedMolecules.FirstOrDefault().Fittness;

            var worstFittness = orderedMolecules.Last().Fittness;


            foreach (Molecule molecule in molecules)
            {
                molecule.Mass = (molecule.Fittness - worstFittness) / (bestFittness - worstFittness);
            }


        }

        public void CalculateFitness(IEnumerable<Molecule> molecules)
        {
            foreach (Molecule agent in molecules)
            {
                agent.Fittness = Function.Calculate(agent.Position);
            }
        }

        private List<Molecule> GenerateRandomAgents()
        {
            List<Molecule> agents = new List<Molecule>();
            for (int i = 0; i < NumAgents; i++)
            {
                var agent = new Molecule
                {
                    Position = new double[NumDimensions],
                    Velocity = new double[NumDimensions]
                };

                for (int j = 0; j < NumDimensions; j++)
                {
                    agent.Position[j] = MinSearchValue + (MaxSearchValue - MinSearchValue) * rand.NextDouble();
                }

                agents.Add(agent);
            }
            return agents;
        }
    }

    public class CustomIteration : ICloneable 
    {
        public List<Molecule> Agents;
        public double Temperature;


        public object Clone()
        {
            return new CustomIteration
            {
                Agents = this.Agents.Select(a => (Molecule)a.Clone()).ToList()
            };
        }
    }
}
