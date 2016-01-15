using System;
using System.Text;
using System.Runtime.Serialization;

namespace SharedObjects
{
    [DataContract]
    public class PublicKey
    {
        [DataMember]
        public byte[] Exponent { get; set; }
        [DataMember]
        public byte[] Modulus { get; set; }
    }
}
