using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub;

namespace CommSubTesting
{
    public class DummyRuntimeOptions : RuntimeOptions
    {
        public override void SetDefaults()
        {
            Registry = "127.0.0.1:12000";
            ANumber = "A00000001";
            FirstName = "The";
            LastName = "Instructor";
            Alias = "Ins";
            MinPortNullable = 12000;
            MaxPortNullable = 12999;
            TimeoutNullable = 3000;
            AutoStart = false;
            RetriesNullable = 3;
        }
    }
}
