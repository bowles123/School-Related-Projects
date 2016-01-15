using System;
using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.StreamMessages
{
    [DataContract]
    public class KeyInfo : StreamMessage
    {
        [DataMember]
        public int ProcessId { get; set; }

        [DataMember]
        public byte[] PublicKeyExponent { get; set; }

        [DataMember]
        public byte[] PublicKeyModulus { get; set; }
    }
}
