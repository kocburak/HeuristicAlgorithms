using System;
using System.Linq;
using System.Text;

namespace HeuristicAlgorithms.GBMO
{
    public class Molecule : ICloneable
    {

        public double[] Position;
        public double[] Velocity;
        public double Mass;
        public double Fittness;

        public object Clone()
        {
            return new Molecule
            {
                Mass = this.Mass,
                Fittness = this.Fittness,
                Position = this.Position.Select(p => p).ToArray(),
                Velocity = this.Velocity.Select(p => p).ToArray(),
            };
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Agent => \nFittness : ");


            builder.Append(Fittness);
            builder.Append("\n");
            for (int i = 0; i < Position.Length; i++)
            {

                builder.Append("x[");
                builder.Append(i);
                builder.Append("] = ");
                builder.Append(Position[i]);
                builder.Append(" ");
                builder.Append("\n");
            }



            return builder.ToString();
        }
    }
}
