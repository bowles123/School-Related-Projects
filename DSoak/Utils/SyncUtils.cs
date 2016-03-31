using System;
using System.Threading;

namespace Utils
{
    public static class SyncUtils
    {
        public static bool WaitForCondition(Func<bool> condition, int timeout = 1000, int step = 100)
        {
            bool result = false;

            int remainingTime = timeout;
            while (remainingTime > 0 && !(result=condition()))
            {
                Thread.Sleep(step);
                remainingTime -= step;
            }

            return result;
        }
    }
}