using HeuristicAlgorithms.GSA;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;

namespace HeuristicAlgorithms.ICA
{
    public class ImperialistCompetitiveAlgorithm
    {
        public IList<Decade> Iterations;
        public int NumCountry;
        public int NumImperialist;
        public int MaxIteration;
        public int NumDimensions;
        public double MinSearchValue;
        public double MaxSearchValue;
        public IFitFunction Function;

        private readonly Random rand = new Random();

        private double AssimilationCoeff = 2;

        public void FindSolution()
        {
            List<Country> Countries = GenerateRandomCountry();

            CalcultateCosts(Countries);

            CalculateNormalizedCosts(Countries);

            CalculateNormalizedPower(Countries);

            IEnumerable<Country> Imperialists = SelectImperialists(Countries);

            AssignCountriesToImperialists(Countries, Imperialists);

            MoveColonies(Imperialists);

            //End First Iteration


            CalcultateCosts(Imperialists);
            Imperialists = SwapIfColonyBetterThanImperialist(Imperialists);

        }

        private IEnumerable<Country> SwapIfColonyBetterThanImperialist(IEnumerable<Country> Imperialists)
        {
            var newImperialists = new List<Country>();
            foreach (Country imperialist in Imperialists)
            {
                newImperialists.Add(imperialist);
            }

            foreach (Country imperialist in Imperialists)
            {

                CalcultateCosts(imperialist.Colonies);

                Country nextImperialist = null;
                foreach (Country colony in imperialist.Colonies)
                {
                    if (nextImperialist == null || nextImperialist.Cost < colony.Cost)
                    {
                        nextImperialist = colony;
                    }
                }

                if (nextImperialist != null)
                {
                    imperialist.Colonies.Remove(nextImperialist);
                    newImperialists.Remove(imperialist);

                    nextImperialist.Colonies = imperialist.Colonies;
                    newImperialists.Add(nextImperialist);
                }
            }
            return newImperialists;
        }

        private void MoveColonies(IEnumerable<Country> Imperialists)
        {
            foreach (Country imperialist in Imperialists)
            {
                foreach (Country colony in imperialist.Colonies)
                {
                    for (int i = 0; i < NumDimensions; i++)
                    {
                        var positionDiff = imperialist.Position[i] - colony.Position[i];

                        colony.Position[i] = colony.Position[i] + AssimilationCoeff * rand.NextDouble() * positionDiff;

                    }

                }
            }
        }

        private void AssignCountriesToImperialists(List<Country> Colonies, IEnumerable<Country> Imperialists)
        {
            var totalNumColonies = NumCountry - NumImperialist;
            //Assign colonies to Emperialists randomly
            foreach (Country imperialist in Imperialists)
            {

                var numOfColonies = Math.Round(imperialist.NormalizedPower * totalNumColonies);

                for (int i = 0; i < numOfColonies; i++)
                {

                    var randomColony = Colonies[rand.Next(0, Colonies.Count - 1)];

                    imperialist.Colonies.Add(randomColony);
                    Colonies.Remove(randomColony);
                }
            }
        }

        private IEnumerable<Country> SelectImperialists(List<Country> Colonies)
        {

            //initial Emperialists
            var Imperialists = Colonies.OrderByDescending(c => c.NormalizedPower).Take(NumImperialist); //TODO: minimization mı olduğu burda belli oluyor. Şuan maximizaiton

            foreach (Country imperialist in Imperialists)
            {
                Colonies.Remove(imperialist);
            }

            return Imperialists;
        }

        private List<Country> GenerateRandomCountry()
        {
            List<Country> colonies = new List<Country>();
            for (int i = 0; i < NumCountry; i++)
            {
                var colony = new Country
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


        public void CalcultateCosts(IEnumerable<Country> countries)
        {
            foreach (Country agent in countries)
            {
                agent.Cost = Function.Calculate(agent.Position);
            }

        }

        private static void CalculateNormalizedPower(List<Country> countries)
        {
            var sumOfNormalizedCost = countries.Max(a => a.NormalizedCost);

            foreach (Country agent in countries)
            {
                agent.NormalizedPower = Math.Abs(agent.NormalizedCost / sumOfNormalizedCost);
            }
        }

        private static void CalculateNormalizedCosts(List<Country> countries)
        {
            var maxCost = countries.Max(a => a.Cost);

            foreach (Country agent in countries)
            {
                agent.NormalizedCost = agent.Cost - maxCost;
            }
        }
    }

    public class Decade : ICloneable
    {
        public List<Country> Colonies;
        public List<Country> Imperialists;

        public object Clone()
        {
            return new Decade
            {
                Colonies = this.Colonies.Select(a => (Country)a.Clone()).ToList(),
                Imperialists = this.Colonies.Select(a => (Country)a.Clone()).ToList()
            };
        }
    }
}
