using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SharedObjects
{
    [DataContract]
    public class Penny : SharedResource
    {
        public Penny Clone()
        {
            return MemberwiseClone() as Penny;
        }

        public override string ToString()
        {
            return string.Format("Penny #{0}", Id);
        }

    }
}
