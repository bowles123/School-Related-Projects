using System.Runtime.Serialization;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class NextIdReply : Reply
    {
        static NextIdReply() { Register(typeof(NextIdReply)); }

        [DataMember]
        public int NextId { get; set; }
        [DataMember]
        public int NumberOfIds { get; set; }
    }
}
