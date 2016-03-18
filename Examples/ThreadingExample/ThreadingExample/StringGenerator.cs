using System;
using System.Threading;

namespace ThreadingExample
{
    public class StringGenerator
    {
        private static readonly Random Randomizer = new Random();
        private bool _keepGoing;

        public string Label { get; set; }
        public int NextWorkItemId { get; set; }
        public int NumberToCreate { get; set; }
        public WorkQueue MyWorkQueue { get; set; }

        public void Start()
        {
            _keepGoing = true;

            if (NextWorkItemId <= 0)
                NextWorkItemId = 1;

            if (NumberToCreate <= 0)
                NumberToCreate = 100;

            ThreadPool.QueueUserWorkItem(Process, null);
        }

        public void Stop()
        {
            _keepGoing = false;
        }

        public void Process(object state)
        {
            while (_keepGoing)
            {
                string newString = String.Empty;
                int length = Randomizer.Next(100, 1000);

                // SLOWLY and INEFFECIENTLY create a string of that length
                for (int i = 0; i < length; i++)
                {
                    int randomAscii = Randomizer.Next(65, 91);
                    char randomChar = Convert.ToChar(randomAscii);
                    string randomString = Convert.ToString(randomChar);
                    newString += randomString;
                    Thread.Sleep(5);
                }

                WorkItem item = new WorkItem() {Id = NextWorkItemId++, InitialString = newString};
                Console.WriteLine("{0, -5} {1,-25} {2}: {3}...{4}",
                                    Label,
                                    "Generated string for item",
                                    item.Id,
                                    item.InitialString.Substring(0, 15),
                                    item.InitialString.Substring(item.InitialString.Length - 15));

                MyWorkQueue.Enqueue(item);

            }
        }
    }
}
