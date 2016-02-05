using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public delegate void StringMethod(string message);

    public static class HelperFunctions
    {
        public static Random randomGenerator = new Random();
        public static object randomLock = new object();

        public static Int32 NextRandom(Int32 min, Int32 max)
        {
            return randomGenerator.Next(min, max);
        }

        public static double RandomDouble(double min, double max, int percision)
        {
            double p = Math.Pow(10, percision);
            Int32 minInt = Convert.ToInt32(min * p);
            Int32 maxInt = Convert.ToInt32(max * p) + 1;
            return randomGenerator.Next(minInt, maxInt) / p;
        }

        public static bool RandomBool(double probablyForTrue)
        {
            double tmp = randomGenerator.NextDouble();
            return (tmp < probablyForTrue);
        }

        public static string ComputeNthLabel(Int32 n)
        {
            string prefix = (n < 0) ? "-" : string.Empty;
            string suffix = "th";

            n = Math.Abs(n);
            Int32 mod10 = (n % 10);
            if (mod10 == 1 && n != 11)
                suffix = "st";
            else if (mod10 == 2 && n != 12)
                suffix = "nd";
            else if (mod10 == 3 && n != 13)
                suffix = "rd";

            return string.Format("{0}{1}{2}", prefix, n, suffix);
        }

        public static string ByteToStringDisplay(byte[] data)
        {
            string result = string.Empty;
            if (data == null)
                result = "null";
            else
            {
                foreach (byte b in data)
                    result += string.Format(" {0,3}", b.ToString("D3"));
            }
            return result;
        }
    }
}
