using System;
using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class JoinGameRequest : Request
    {
        [DataMember]
        public Int32 GameId { get; set; }
        [DataMember]
        public ProcessInfo Player { get; set; }
    }
}
