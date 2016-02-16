using System;

namespace Utils
{
    public static class RandomChooser
    {
        private static readonly Random Randomizer = new Random();

        public static int IntChoice(int min, int exclusiveMax)
        {
            return Randomizer.Next(min, exclusiveMax);
        }

        public static bool BoolChoice()
        {
            return BoolChoice(.50);
        }

        public static bool BoolChoice(double probablyOfTrue)
        {
            double randomValue = Randomizer.NextDouble();
            return randomValue <= probablyOfTrue;
        }

    }
}
