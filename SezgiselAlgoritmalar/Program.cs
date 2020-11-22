using System;
using System.Collections.Generic;
using System.Linq;
namespace SezgiselAlgoritmalar
{
    class Program
    {
        static void Main(string[] args)
        {
            GravitaionalSearchAlgorithm algorithm = new GravitaionalSearchAlgorithm(new f37(), 10, 30, 5000, -5.12, 5.12);
            algorithm.FindSolution();
            var last = algorithm.Iterations[algorithm.Iterations.Count - 1].Agents;
            Console.WriteLine("Best Fittness: " + algorithm.Iterations.Min(A => A.Agents.Min(q => q.Fittness)));
            Console.WriteLine("Best Agent: " + algorithm.Iterations.OrderBy(i => i.Agents.Min(q => q.Fittness)).FirstOrDefault().Agents.OrderBy(a => a.Fittness).FirstOrDefault().ToString());
            Console.ReadKey();
        }


    }

    class f37 : IFitFunction //�Drop wave� function ++        i=1,2...    f(x)=-1.0  @x=(0,0)   -5.12<x[i]<5.12
    {
        public double Calculate(double[] input)
        {
            //��z�m� istenen fonksiyon	
            double r = Math.PI / 180;
            double top1 = -(1 + Math.Cos((12 * Math.Sqrt(input[0] * input[0] + input[1] * input[1])))) / (0.5 * (input[0] * input[0] + input[1] * input[1]) + 2.0);
            return top1;
        }
    }
}
