using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadingExample
{
    public class WorkItem
    {
        public int Id { get; set; }
        public string InitialString { get; set; }
        public string ReversedString { get; set; }
    }
}
