using System;
using System.Runtime.Serialization;

namespace Messages.StreamMessages
{
    [DataContract]
    public class NotInGame: StreamMessage
    {
        [DataMember]
        public int ProcessId { get; set; }
    }
}
