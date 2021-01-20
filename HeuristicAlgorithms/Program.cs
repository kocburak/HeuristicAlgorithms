using HeuristicAlgorithms.GBMO;
using HeuristicAlgorithms.GSA;
using HeuristicAlgorithms.ICA;
using System;
using System.Collections.Generic;
using System.Linq;
namespace HeuristicAlgorithms
{
    class Program
    {
        static void Main(string[] args)
        {

            //Todo: Tüm algoritmaları minimization yap

            /* GravitationalSearchAlgorithm algorithm = new GravitationalSearchAlgorithm(new f37(), 10, 2, 5000, -5.12, 5.12);
            algorithm.FindSolution(); 

            var last = algorithm.Iterations[algorithm.Iterations.Count - 1].Agents;
            Console.WriteLine("Best Fittness: " + algorithm.Iterations.Min(A => A.Agents.Min(q => q.Fittness)));
            Console.WriteLine("Best Agent: " + algorithm.Iterations.OrderBy(i => i.Agents.Min(q => q.Fittness)).FirstOrDefault().Agents.OrderBy(a => a.Fittness).FirstOrDefault().ToString());
            Console.ReadKey();*/



            /*ImperialistCompetitiveAlgorithm algorithm = new ImperialistCompetitiveAlgorithm(new f37(), 10, 2, 2, 5000, -5.12, 5.12);
            algorithm.FindSolution();

            var last = algorithm.Iterations[algorithm.Iterations.Count - 1].Imperialists;
            Console.WriteLine("Best Fittness: " + algorithm.Iterations.Max(A => A.Imperialists.Max(q => q.Colonies.Max(w => w.Cost))));
            Console.WriteLine("Best Agent: " + algorithm.Iterations.OrderByDescending(i => i.Imperialists.Max(b => b.Cost)).FirstOrDefault().Imperialists.OrderByDescending(a => a.Cost).FirstOrDefault());
            Console.ReadKey();*/

            while (true)
            {

                //Maximization
                   ImperialistCompetitiveAlgorithm algorithm = new ImperialistCompetitiveAlgorithm(new f1(), OptimizationType.Maximization, 80, 5, 2, 1000, -100, 100);
                   algorithm.FindSolution();
                   var last = algorithm.Iterations[algorithm.Iterations.Count - 1].Imperialists;
                   Console.WriteLine("Best Fittness: " + algorithm.Iterations.Max(A => A.Imperialists.Max(q => q.Cost > q.Colonies.Max(w => w.Cost) ? q.Cost : q.Colonies.Max(w => w.Cost))));
                   Console.WriteLine("Best Agent: " + algorithm.Iterations.OrderByDescending(i => i.Imperialists.Max(b => b.Cost)).FirstOrDefault().Imperialists.OrderByDescending(a => a.Cost).FirstOrDefault());

                   double best = double.MinValue;
                   int i = 0;
                   foreach (Decade iteration in algorithm.Iterations)
                   {
                       var iterationBest = iteration.Imperialists.Max(q => q.Cost > q.Colonies.Max(w => w.Cost) ? q.Cost : q.Colonies.Max(w => w.Cost));
                       if (best < iterationBest)
                       {

                           best = iterationBest;
                           Console.Write($"({i},{String.Format("{0:0.0000}", -1 *best)})");
                       }

                       i++;
                   }


                  /*GravitationalSearchAlgorithm algorithm = new GravitationalSearchAlgorithm(new f3(), OptimizationType.Minimization, 80, 2, 1000, -100,100);

                  algorithm.FindSolution();
                  var last = algorithm.Iterations[algorithm.Iterations.Count - 1].Agents;
                  Console.WriteLine("Best Fittness: " + algorithm.Iterations.Min(A => A.Agents.Min(q => q.Fittness)));
                  Console.WriteLine("Best Agent: " + algorithm.Iterations.SelectMany(a => a.Agents).OrderBy(i => i.Fittness).FirstOrDefault()); 

                   double best = double.MaxValue;
                   int i = 0;
                   foreach (Iteration<Agent> iteration in algorithm.Iterations)
                   {
                       var iterationBest = iteration.Agents.Min(q => q.Fittness);
                       if (best > iterationBest)
                      {
                          best = iterationBest;
                          Console.Write($"({i},{String.Format("{0:0.0000}", best)})");
                      }
                       i++;
                   }*/
                  
                  


              /* GasesBrownianMotionOptimization algorithm = new GasesBrownianMotionOptimization(new f3(),OptimizationType.Minimization , 80, 2, 1000, -100, 100);

                algorithm.FindSolution();
                var last = algorithm.Iterations[algorithm.Iterations.Count - 1].Agents;
                Console.WriteLine("Best Fittness: " + algorithm.Iterations.Min(A => A.Agents.Min(q => q.Fittness)));
                Console.WriteLine("Best Agent: " + algorithm.Iterations.SelectMany(a => a.Agents).OrderBy(i => i.Fittness).FirstOrDefault());

                double best = double.MaxValue;
                int i = 0;
                foreach (CustomIteration iteration in algorithm.Iterations)
                {
                    var iterationBest = iteration.Agents.Min(q => q.Fittness);
                    if (best > iterationBest)
                    {
                        best = iterationBest;
                        Console.Write($"({i},{String.Format("{0:0.0000}", best)})");
                    }
                    i++;
                }

                */



                /*  var last = algorithm.Iterations[algorithm.Iterations.Count - 1].Agents;
                   Console.WriteLine("Best Fittness: " + algorithm.Iterations.Max(A => A.Agents.Max(q => q.Fittness)));
                  Console.WriteLine("Best Agent: " + algorithm.Iterations.SelectMany(a => a.Agents).OrderByDescending(i => i.Fittness).FirstOrDefault());*/






                Console.ReadKey();
            }
        }


    }

    class f37 : IFitFunction 
    {
        public double Calculate(double[] input)
        {
            //��z�m� istenen fonksiyon	
           // double r = Math.PI / 180;
            double top1 = /* - */(1 + Math.Cos((12 * Math.Sqrt(input[0] * input[0] + input[1] * input[1])))) / (0.5 * (input[0] * input[0] + input[1] * input[1]) + 2.0);
            return top1;
        }
    }
    class f3 : IFitFunction
    {
        public double Calculate(double[] input)
        {
            double output = 0;

            for (int i=0; i < input.Length-1;i++)
            {
                output += 100 * Math.Pow(input[i + 1] - input[i] * input[i], 2) + Math.Pow(input[i] - 1, 2);
            }

            return -1* output;
        }
    }

    class f2 : IFitFunction
    {
        public double Calculate(double[] input)
        {
            double output = 0;

            foreach (double x in input)
            {

                output += x * x;
                output -= 10 * Math.Cos(2 * Math.PI * x);

                output += 10;
            }

            return -1 *output;
        }
    }

    class f1 : IFitFunction 
    {
        public double Calculate(double[] input)
        {
            double output = 0;

            foreach (double x in input)
                output += x * x;

            return -1* output;
        }
    }
}
