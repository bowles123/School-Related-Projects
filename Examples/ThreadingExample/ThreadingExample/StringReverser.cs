using System;
using System.Threading;

namespace ThreadingExample
{
    public class StringReverser
    {
        private bool _keepGoing;

        public WorkQueue MyWorkQueue { get; set; }

        private Thread _myThread;

        public void Start()
        {
            _keepGoing = true;

            _myThread = new Thread(Process);
            _myThread.Start();
        }

        public void Stop()
        {
            _keepGoing = false;
            _myThread.Join();
        }

        public void Process()
        {
            while (_keepGoing)
            {
                WorkItem item = MyWorkQueue.Dequeue(1000);
                if (item!=null)
                    ProcessWorkItem(item);

                Thread.Sleep(0);
            }
        }

        private void ProcessWorkItem(WorkItem item)
        {
            string reversedString = String.Empty;

            // Slowing and ineffeciently reverse the string
            foreach (char c in item.InitialString)
            {
                reversedString = c + reversedString;
                Thread.Sleep(10);
            }

            item.ReversedString = reversedString;
            Console.WriteLine("{0,-25} {1}: {2}...{3}",
                    "Reversed string for item",
                    item.Id,
                    item.ReversedString.Substring(0, 15),
                    item.ReversedString.Substring(item.ReversedString.Length-15));
        }
    }
}
