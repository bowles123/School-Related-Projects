using System;
using System.Threading;

namespace TwoThreads
{
    public class Resource
    {
        public string Name { get; set; }
        public string Data { get; set; }

        public void DoWork()
        {
            Console.WriteLine("{0}: Do work on {1}", Thread.CurrentThread.ManagedThreadId, Name);
            Thread.Sleep(100);
        }

        public void DoMoreWork(Resource otherResource)
        {
            if (otherResource != null)
            {
                Console.WriteLine("{0}: Do work on {1} and {2}", Thread.CurrentThread.ManagedThreadId, Name, otherResource.Name);
                Thread.Sleep(1000);
            }
        }
    }
}
