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

        private double AssimilationCoeff = 4; // Beta
        private double RevolutionRate = 0.02;
        private double zeta = 0.02; // Total Cost of Empire = Cost of Imperialist + Zeta * mean(Cost of All Colonies)

        public ImperialistCompetitiveAlgorithm(IFitFunction function, int numCountry, int numImperialist, int numDimensions, int maxIteration, double minSearchValue, double maxSearchValue)
        {
            MaxIteration = maxIteration;
            NumCountry = numCountry;
            NumDimensions = numDimensions;
            MinSearchValue = minSearchValue;
            MaxSearchValue = maxSearchValue;
            Function = function;
            NumImperialist = numImperialist;

        }

        public void FindSolution()
        {
            List<Country> Countries = GenerateRandomCountry(NumCountry);


            CalculateCosts(Countries);

            List<Empire> Imperialists = SelectImperialists(Countries);

            CalculatePower(Imperialists);

            CalculateNormalizedPower(Imperialists);

            AssignCountriesToImperialists(Countries, Imperialists);

            Iterations = new List<Decade>();

            while (Iterations.Count < MaxIteration)
            {

                CalculateCosts(Imperialists);

                CalculatePower(Imperialists);

                CalculateNormalizedPower(Imperialists);

                Iterations.Add(new Decade()
                {
                    Imperialists = Imperialists.Select(i => (Empire) i.Clone()).ToList()
                });


                MoveColonies(Imperialists);

                Imperialists = SwapIfColonyBetterThanImperialist(Imperialists);

                Revolution(Imperialists);

                ImperialisticCompetition(Imperialists);

                //CalcultateCosts(Imperialists);


            }


        }

        private void ImperialisticCompetition(IList<Empire> Imperialists)
        {
            foreach (Empire imperialist in Imperialists)
            {
                imperialist.PowerOfEmpire = imperialist.Cost + zeta * imperialist.Colonies.Select(c => c.Cost).Average();
            }


            foreach (Empire imperialist in Imperialists)
            {
                imperialist.NormalizedPowerOfEmpire = Imperialists.Select(i => i.PowerOfEmpire).Max() - imperialist.PowerOfEmpire;
            }

            foreach (Empire imperialist in Imperialists)
            {
                imperialist.PossessionProbability = (imperialist.NormalizedPowerOfEmpire / Imperialists.Select(i => i.NormalizedPowerOfEmpire).Max()) - rand.NextDouble();
            }

            var weakestEmpire = Imperialists.OrderBy(i => i.PowerOfEmpire).FirstOrDefault();

            var weakestColonyOfWeakestEmpire = weakestEmpire.Colonies.OrderBy(c => c.Cost).FirstOrDefault(); //TODO: minimization mı olduğu burda belli oluyor. Şuan maximizaiton


            var strongestEmpire = Imperialists.OrderByDescending(i => i.PossessionProbability).FirstOrDefault();

            weakestEmpire.Colonies.Remove(weakestColonyOfWeakestEmpire);
            strongestEmpire.Colonies.Add(weakestColonyOfWeakestEmpire);


            //Elimination of the weakest
            if (weakestEmpire.Colonies.Count == 0)
            {
                Imperialists.Remove(weakestEmpire);
                strongestEmpire.Colonies.Add(weakestEmpire);
            }


        }

        private void Revolution(IEnumerable<Empire> Imperialists)
        {
            foreach (Empire imperialist in Imperialists)
            {

                var numRevolutionColonies = (int)Math.Round(RevolutionRate * imperialist.Colonies.Count);

                for (int i = 0; i < numRevolutionColonies; i++)
                {

                    imperialist.Colonies.RemoveAt(rand.Next(0, imperialist.Colonies.Count - 1));

                }

                var newCountries = GenerateRandomCountry(numRevolutionColonies);

                imperialist.Colonies.AddRange(newCountries);

            }
        }

        private List<Empire> SwapIfColonyBetterThanImperialist(IEnumerable<Empire> Imperialists)
        {
            var newImperialists = new List<Empire>();
            foreach (Empire imperialist in Imperialists)
            {
                newImperialists.Add(imperialist);
            }

            foreach (Empire imperialist in Imperialists)
            {
                Empire nextImperialist = null;
                foreach (Country colony in imperialist.Colonies)
                {
                    if (nextImperialist == null || nextImperialist.Cost < colony.Cost)
                    {
                        nextImperialist = new Empire(colony);
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
            foreach (Empire imperialist in Imperialists)
            {
                foreach (Country colony in imperialist.Colonies)
                {
                    for (int i = 0; i < NumDimensions; i++)
                    {
                        var positionDiff = imperialist.Position[i] - colony.Position[i];

                        colony.Position[i] = colony.Position[i] + AssimilationCoeff * rand.NextDouble() * positionDiff;

                    }

                }

                CalculateCosts(imperialist.Colonies);
            }
        }

        private void AssignCountriesToImperialists(List<Country> Colonies, IEnumerable<Empire> Imperialists)
        {

            var totalNumColonies = NumCountry - NumImperialist;
            //Assign colonies to Emperialists randomly
            foreach (Empire imperialist in Imperialists)
            {
                imperialist.Colonies = new List<Country>();

                var numOfColonies = (int)Math.Round(imperialist.NormalizedPower * totalNumColonies);

                for (int i = 0; i < numOfColonies; i++)
                {
                    if (Colonies.Count == 0)
                        Colonies = GenerateRandomCountry(numOfColonies - i);

                    var randomColony = Colonies[rand.Next(0, Colonies.Count - 1)];

                    imperialist.Colonies.Add(randomColony);
                    Colonies.Remove(randomColony);
                }
            }

            if (Colonies.Count > 0)
                Imperialists.Last().Colonies.AddRange(Colonies);

        }

        private List<Empire> SelectImperialists(List<Country> Colonies)
        {

            //initial Emperialists
            var Imperialists = Colonies.OrderByDescending(c => c.Cost).Take(NumImperialist); //TODO: minimization mı olduğu burda belli oluyor. Şuan maximizaiton

            var Imp = new List<Empire>();

            foreach (Country imperialist in Imperialists)
            {
                Colonies.Remove(imperialist);
                Imp.Add(new Empire(imperialist));
            }

            return Imp;
        }

        private List<Country> GenerateRandomCountry(int count)
        {
            List<Country> colonies = new List<Country>();
            for (int i = 0; i < count; i++)
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


        public void CalculateCosts(IEnumerable<Country> countries)
        {
            foreach (Country agent in countries)
            {
                agent.Cost = Function.Calculate(agent.Position);
            }

        }

        private static void CalculateNormalizedPower(IEnumerable<Empire> countries)
        {
            var sumOfNormalizedCost = countries.Sum(a => a.Power);

            foreach (Empire agent in countries)
            {
                agent.NormalizedPower = Math.Abs(agent.Power / sumOfNormalizedCost);
            }
        }

        private static void CalculatePower(IEnumerable<Empire> empires)
        {
            var maxCost = empires.Max(a => a.Cost);

            foreach (Empire agent in empires)
            {
                if (maxCost > 0)
                    agent.Power = 1.3 * maxCost - agent.Cost;
                else
                    agent.Power = 0.7 * maxCost - agent.Cost;
            }
        }
    }
}
