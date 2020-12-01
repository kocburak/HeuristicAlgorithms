using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicAlgorithms.ICA
{
    public class Country : ICloneable
    {
        public double[] Position;
        public double Cost;


        public object Clone()
        {
            return new Country
            {
                Cost = this.Cost,
                Position = this.Position.Select(p => p).ToArray()
            };
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Agent => \nFittness : ");


            builder.Append(Cost);
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

    public class Empire : Country
    {
        public double Power;
        public double NormalizedPower;
        public double PowerOfEmpire;
        public double NormalizedPowerOfEmpire;
        public double PossessionProbability;
        public List<Country> Colonies;

        public Empire()
        {
        }


        public Empire(Country country)
        {
            Cost = country.Cost;
            Position = country.Position.Select(p => p).ToArray();
        }


        public new object Clone()
        {
            return new Empire
            {
                Cost = this.Cost,
                Power = this.Power,
                NormalizedPower = this.NormalizedPower,
                PowerOfEmpire = this.PowerOfEmpire,
                NormalizedPowerOfEmpire = this.NormalizedPowerOfEmpire,
                PossessionProbability = this.PossessionProbability,
                Position = this.Position.Select(p => p).ToArray(),
                Colonies = this.Colonies?.Select(p => p).ToList()
            };
        }
    }
}
