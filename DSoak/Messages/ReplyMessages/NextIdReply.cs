using System.Runtime.Serialization;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class NextIdReply : Reply
    {
        [DataMember]
        public int NextId { get; set; }
    }
}
