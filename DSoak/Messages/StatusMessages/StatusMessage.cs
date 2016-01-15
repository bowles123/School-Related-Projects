using System;
using System.Runtime.Serialization;

using Messages;

namespace Messages.StatusMessages
{
    [DataContract]
    public class StatusMessage : Message
    {
        [DataMember]
        public int GameId { get; set; }
    }
}
