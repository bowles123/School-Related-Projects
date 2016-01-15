using System;
using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.StreamMessages
{
    [DataContract]
    public class InGame: StreamMessage
    {
        [DataMember]
        public ProcessInfo Process { get; set; }
    }
}
