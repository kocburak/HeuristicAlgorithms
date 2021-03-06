﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicAlgorithms.GSA
{
    public class GravitationalSearchAlgorithm
    {

        public IList<Iteration<Agent>> Iterations;
        public int MaxIteration;
        public int NumAgents;
        public int NumDimensions;
        public double MinSearchValue;
        public double MaxSearchValue;
        public IFitFunction Function;
        public OptimizationType OptimizationType;

        private readonly Random rand = new Random();

        public GravitationalSearchAlgorithm(IFitFunction function, OptimizationType optimizationType, int numAgents, int numDimensions, int maxIteration, double minSearchValue, double maxSearchValue)
        {
            MaxIteration = maxIteration;
            NumAgents = numAgents;
            NumDimensions = numDimensions;
            MinSearchValue = minSearchValue;
            MaxSearchValue = maxSearchValue;
            Function = function;
            OptimizationType = optimizationType;

        }


        //https://www.sciencedirect.com/science/article/pii/S0142061514002440#b0140

        public double CalculateEuclidianDistance(Agent a, Agent b)
        {
            double sumOfSquares = 0;
            for (int i = 0; i < a.Position.Length; i++)
            {
                sumOfSquares += Math.Pow(a.Position[i] - b.Position[i], 2);
            }
            return Math.Sqrt(sumOfSquares);
        }

        public double CalculateGravitationalConstant()
        {
            return 100.0 * Math.Exp(-20.0 * Iterations.Count/ MaxIteration); //Equation 28
        }

        public void CalculateFitness(Iteration<Agent> iteration)
        {
            foreach (Agent agent in iteration.Agents)
            {
                agent.Fittness = Function.Calculate(agent.Position);
            }
        }

        public void FindSolution()
        {
            var firstIteration = new Iteration<Agent>
            {
                Agents = GenerateRandomAgents()
            };
            Iterations = new List<Iteration<Agent>>
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

                CalculateFitness(currentIteration);

                IEnumerable<Agent> orderedAgents = null;

                if (OptimizationType == OptimizationType.Maximization)
                {
                    orderedAgents = currentIteration.Agents.OrderByDescending(a => a.Fittness);
                }
                else if (OptimizationType == OptimizationType.Minimization)
                {
                    orderedAgents = currentIteration.Agents.OrderBy(a => a.Fittness);
                }

                currentIteration.Agents = orderedAgents.ToList();

                var bestFittnessAgent = orderedAgents.FirstOrDefault();

                var worstFittnessAgent = orderedAgents.Last();

                CalculateMass(currentIteration, bestFittnessAgent.Fittness, worstFittnessAgent.Fittness);

                double gravitationalConst = CalculateGravitationalConstant();

                CalculateGravField(currentIteration, gravitationalConst);

                foreach (Agent agent in currentIteration.Agents)
                {
                    // Console.WriteLine(agent.ToString());

                }


                Iterations.Add((Iteration<Agent>)currentIteration.Clone());

                currentIteration = Iterations[Iterations.Count - 1];

                Movements(currentIteration);

            }

        }

        public void CalculateGravField(Iteration<Agent> iteration, double gravitationalConst)
        {


            double epsilon = 0.001;



            for (int i = 0; i < NumAgents - NumAgents * Iterations.Count / MaxIteration; i++)
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
                            agent1.TotalGravitaionalField[k] += rand.NextDouble() * agent2.Mass * ((agent2.Position[k] - agent1.Position[k]) / (R + epsilon));
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

        private void CalculateMass(Iteration<Agent> iteration, double bestFittness, double worstFittness)
        {
            foreach (Agent agent in iteration.Agents)
            {
                agent.Mass = (agent.Fittness - worstFittness) / (bestFittness - worstFittness);
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

        public void Movements(Iteration<Agent> iteration)
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
