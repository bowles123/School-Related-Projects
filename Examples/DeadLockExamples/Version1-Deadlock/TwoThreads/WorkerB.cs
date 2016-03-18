using System;
using System.Threading;

namespace TwoThreads
{
    public class WorkerB : BackgroundThread
    {
        protected override void Process(object state)
        {
            while (KeepGoing)
            {
                lock (ResourceCollection.LockB)
                {
                    Console.WriteLine("{0}: Got access to B", Thread.CurrentThread.ManagedThreadId);
                    ResourceCollection.B.DoWork();
                    lock (ResourceCollection.LockA)
                    {
                        Console.WriteLine("{0}: Got access to A", Thread.CurrentThread.ManagedThreadId);
                        ResourceCollection.B.DoMoreWork(ResourceCollection.A);
                    }
                }
            }
        }
    }
}
