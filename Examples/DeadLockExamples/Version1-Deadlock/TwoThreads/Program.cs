using System;

namespace TwoThreads
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkerA workerA = new WorkerA();
            workerA.Start();

            WorkerB workerB = new WorkerB();
            workerB.Start();

            Console.ReadLine();
        }
    }
}
