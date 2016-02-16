using System;
using System.Threading;

namespace ThreadingExample
{
    public class StringGenerator
    {
        private static readonly Random Randomizer = new Random();
        private bool _keepGoing;

        public int NextWorkItemId { get; set; }
        public WorkQueue MyWorkQueue { get; set; }

        public void Start()
        {
            _keepGoing = true;

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
                Console.WriteLine("{0,-25} {1}: {2}...{3}",
                                    "Generated string for item",
                                    item.Id,
                                    item.InitialString.Substring(0, 15),
                                    item.InitialString.Substring(item.InitialString.Length - 15));

                MyWorkQueue.Enqueue(item);

            }
        }
    }
}
