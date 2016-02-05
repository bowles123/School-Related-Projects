using System;
using System.Runtime.Serialization;

namespace SharedObjects
{
    [DataContract]
    public class Umbrella : SharedResource
    {
        public override string ToString()
        {
            return string.Format("Umbrella #{0}", Id);
        }
    }
}
