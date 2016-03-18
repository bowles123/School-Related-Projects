using System;using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class BalloonReply : Reply
    {
        [DataMember]
        public Balloon Balloon { get; set; }
    }
}
