using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadingExample
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkQueue queue = new WorkQueue();
            StringGenerator gen1 = new StringGenerator() { NextWorkItemId = 1000, MyWorkQueue = queue };
            gen1.Start();

            StringReverser rev1 = new StringReverser() {MyWorkQueue = queue};
            rev1.Start();

            StringReverser rev2 = new StringReverser() { MyWorkQueue = queue };
            rev2.Start();

            Console.WriteLine("Hit ENTER to exit");
            Console.ReadLine();

            gen1.Stop();
            rev1.Stop();
            rev2.Stop();
        }
    }
}
