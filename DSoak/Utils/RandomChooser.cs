using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class RandomChooser
    {
        private static Random randomizer = new Random();

        public static int IntChoice(int min, int exclusiveMax)
        {
            return randomizer.Next(min, exclusiveMax);
        }
        public static bool BoolChoice()
        {
            return BoolChoice(.50);
        }

        public static bool BoolChoice(double probablyOfTrue)
        {
            double randomValue = randomizer.NextDouble();
            return randomValue <= probablyOfTrue;
        }

    }
}
