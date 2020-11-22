using System;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicAlgorithms
{
    public class GravitationalSearchAlgorithm
    {

        public IList<Iteration> Iterations;
        public int MaxIteration;
        public int NumAgents;
        public int NumDimensions;
        public double MinSearchValue;
        public double MaxSearchValue;
        public IFitFunction Function;

        private readonly Random rand = new Random();

        public GravitationalSearchAlgorithm(IFitFunction function, int numAgents, int numDimensions, int maxIteration, double minSearchValue, double maxSearchValue)
        {
            MaxIteration = maxIteration;
            NumAgents = numAgents;
            NumDimensions = numDimensions;
            MinSearchValue = minSearchValue;
            MaxSearchValue = maxSearchValue;
            Function = function;

        }

        public double CalculateEuclidianDistance(Agent a, Agent b)
        {
            double sumOfSquares = 0;
            for (int i = 0; i < a.Position.Length; i++)
            {
                sumOfSquares += Math.Pow(a.Position[i] - b.Position[i], 2);
            }
            return Math.Sqrt(sumOfSquares);
        }

        public void CalculateTotalForceForAgent(Agent agent)
        {

        }

        public double CalculateGravitationalConstant()
        {
            return 100.0 * Math.Exp(-20.0 * (Iterations.Count - 1) / MaxIteration); //Equation 28
        }

        public void CalcultateFitness(Iteration iteration)
        {
            double[] fittness = new double[NumAgents];
            foreach (Agent agent in iteration.Agents)
            {
                agent.Fittness = Function.Calculate(agent.Position);
            }
        }

        public void FindSolution()
        {
            var firstIteration = new Iteration
            {
                Agents = GenerateRandomAgents()
            };
            Iterations = new List<Iteration>
            {
                firstIteration
            };
            var currentIteration = firstIteration;

            while (Iterations.Count <= MaxIteration)
            {
                foreach (Agent agent in currentIteration.Agents)
                {
                    for (int i = 0; i < agent.Position.Length; i++)
                    {
                        if (agent.Position[i] < MinSearchValue || agent.Position[i] > MaxSearchValue)
                        {
                            agent.Position[i] = MinSearchValue + (MaxSearchValue - MinSearchValue) * rand.NextDouble();
                        }
                    }
                }

                CalcultateFitness(currentIteration);

                var bestFittnessAgent = currentIteration.Agents.OrderBy(a => a.Fittness).FirstOrDefault();

                var worstFittnessAgent = currentIteration.Agents.OrderByDescending(a => a.Fittness).FirstOrDefault();

                CalculateMass(currentIteration, bestFittnessAgent.Fittness, worstFittnessAgent.Fittness);

                double gravitationalConst = CalculateGravitationalConstant();

                CalculateGravField(currentIteration, gravitationalConst);

                foreach (Agent agent in currentIteration.Agents)
                {
                    // Console.WriteLine(agent.ToString());

                }


                Iterations.Add((Iteration)currentIteration.Clone());

                currentIteration = Iterations[Iterations.Count - 1];

                Movements(currentIteration);

            }

        }

        public void CalculateGravField(Iteration iteration, double gravitationalConst)
        {
            double epsilon = 0.001;

            for (int i = 0; i < iteration.Agents.Count; i++)
            {
                Agent agent1 = iteration.Agents[i];
                for (int j = 0; j < iteration.Agents.Count; j++)
                {
                    Agent agent2 = iteration.Agents[j];
                    if (j != i)
                    {
                        double R = CalculateEuclidianDistance(agent1, agent2);
                        for (int k = 0; k < NumDimensions; k++)
                        {
                            agent1.TotalGravitaionalField[k] += rand.NextDouble() * agent2.Mass * ((agent2.Position[k] - agent1.Position[k]) / ((R * R) + epsilon));
                        } //Equation 7
                    }
                }
            }

            foreach (Agent agent in iteration.Agents)
            {
                for (int k = 0; k < NumDimensions; k++)
                {
                    agent.TotalGravitaionalField[k] *= gravitationalConst;
                }
            }
        }

        private void CalculateMass(Iteration iteration, double betsFittness, double worstFittness)
        {
            foreach (Agent agent in iteration.Agents)
            {
                agent.Mass = (agent.Fittness - worstFittness) / (betsFittness - worstFittness);
            }

            double totalMass = iteration.Agents.Select(a => a.Mass).Sum();

            foreach (Agent agent in iteration.Agents)
            {
                agent.Mass /= totalMass;
            }

        }

        private List<Agent> GenerateRandomAgents()
        {
            List<Agent> agents = new List<Agent>();
            for (int i = 0; i < NumAgents; i++)
            {
                var agent = new Agent
                {
                    Position = new double[NumDimensions],
                    Velocity = new double[NumDimensions],
                    TotalGravitaionalField = new double[NumDimensions]
                };

                for (int j = 0; j < NumDimensions; j++)
                {
                    agent.Position[j] = MinSearchValue + (MaxSearchValue - MinSearchValue) * rand.NextDouble();
                }

                agents.Add(agent);
            }
            return agents;
        }

        public void Movements(Iteration iteration)
        {

            foreach (Agent agent in iteration.Agents)
            {
                for (int i = 0; i < NumDimensions; i++)
                {
                    agent.Velocity[i] = rand.NextDouble() * agent.Velocity[i] + agent.TotalGravitaionalField[i]; //Equation (11)
                    agent.Position[i] += agent.Velocity[i]; //Equation (12) 	
                }
            }
        }
    }
}
