using System;
using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.StatusMessages
{
    [DataContract]
    public class ReadyToStart : StatusMessage
    {
        [DataMember]
        public PublicEndPoint StreamEP { get; set; }
        [DataMember]
        public GameInfo Game { get; set; }
    }
}
