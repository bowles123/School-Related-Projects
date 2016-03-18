using System;using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class BalloonReply : Reply
    {
        static BalloonReply() { Register(typeof(BalloonReply)); }

        [DataMember]
        public Balloon Balloon { get; set; }
    }
}
