using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoThreads
{
    public static class ResourceCollection
    {
        public static Resource A { get; set; }
        public static Resource B { get; set; }

        public static object LockA { get; set; }
        public static object LockB { get; set; }

        static ResourceCollection()
        {
            A = new Resource() { Name = "A", Data = "1234567890"};
            B = new Resource() { Name = "B", Data = "9876543210"};
            
            LockA = new object();
            LockB = new object();
        }
    }
}
