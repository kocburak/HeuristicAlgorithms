using System;
using System.Collections.Generic;
using System.Text;

namespace HeuristicAlgorithms
{
    public interface IFitFunction
    {
        double Calculate(double[] input);
    }
}
