using System;
using System.Threading;

namespace TwoThreads
{
    public class WorkerA : BackgroundThread
    {
        protected override void Process(object state)
        {
            while (KeepGoing)
            {
                lock (ResourceCollection.LockA)
                {
                    Console.WriteLine("{0}: Got access to A", Thread.CurrentThread.ManagedThreadId);
                    ResourceCollection.A.DoWork();
                    lock (ResourceCollection.LockB)
                    {
                        Console.WriteLine("{0}: Got access to B", Thread.CurrentThread.ManagedThreadId);
                        ResourceCollection.A.DoMoreWork(ResourceCollection.B);                        
                    }
                }
            }
        }
    }
}
