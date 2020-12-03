using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicAlgorithms.GSA;

namespace HeuristicAlgorithms.GBMO
{
    public class GasesBrownianMotionOptimization
    {
        public IList<Iteration<Molecule>> Iterations;
        public int MaxIteration;
        public int NumAgents;
        public int NumDimensions;
        public double MinSearchValue;
        public double MaxSearchValue;
        public IFitFunction Function;


        private readonly Random rand = new Random();


        private double Temperature = 297;
        private double BoltzmanConstant = 1.38e-23;

        public GasesBrownianMotionOptimization(IFitFunction function, int numAgents, int numDimensions, int maxIteration, double minSearchValue, double maxSearchValue)
        {
            MaxIteration = maxIteration;
            NumAgents = numAgents;
            NumDimensions = numDimensions;
            MinSearchValue = minSearchValue;
            MaxSearchValue = maxSearchValue;
            Function = function;
        }


        public void FindSolution()
        {
            var molecules = GenerateRandomAgents();

            CalculateFitness(molecules);

            CalculateMass(molecules);

            var firstIteration = new Iteration<Molecule>
            {
                Agents = molecules
            };

            Iterations = new List<Iteration<Molecule>>
            {
                (Iteration<Molecule>) firstIteration.Clone()
            };

            var currentIteration = firstIteration;


            while (Iterations.Count <= MaxIteration)
            {
                Movements(molecules);

                CalculateFitness(molecules);

                CalculateMass(molecules);

                CalculateTemperature(molecules);

                Iterations.Add((Iteration<Molecule>)currentIteration.Clone());

                currentIteration = Iterations[Iterations.Count - 1];
            }
        }

        private void CalculateTemperature(List<Molecule> molecules)
        {
            var avareageFittness = molecules.Select(m => m.Fittness).Average();

            Temperature -= (1 / avareageFittness);
        }

        public void Movements(IEnumerable<Molecule> molecules)
        {
            foreach (Molecule agent in molecules)
            {
                for (int i = 0; i < NumDimensions; i++)
                {
                    //TODO: Bu böyle çalışmaz.
                    agent.Velocity[i] = agent.Velocity[i] + Math.Sqrt(3 * BoltzmanConstant * Temperature / agent.Mass);
                    agent.Position[i] += agent.Velocity[i];

                    agent.Position[i] += 0.2 - (0.5 / (2 * Math.PI)) * Math.Sin(2 * Math.PI * agent.Position[i]);
                }
            }
        }

        private void CalculateMass(IEnumerable<Molecule> molecules)
        {

            var bestFittness = molecules.OrderByDescending(a => a.Fittness).FirstOrDefault().Fittness;

            var worstFittness = molecules.OrderBy(a => a.Fittness).FirstOrDefault().Fittness;


            foreach (Molecule molecule in molecules)
            {
                molecule.Mass = (molecule.Fittness - worstFittness) / (bestFittness - worstFittness);
            }

            double totalMass = molecules.Select(a => a.Mass).Sum();

            foreach (Molecule molecule in molecules)
            {
                molecule.Mass /= totalMass;
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
}
