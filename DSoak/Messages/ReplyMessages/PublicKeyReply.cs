using System.Runtime.Serialization;
using SharedObjects;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class PublicKeyReply : Reply
    {
        [DataMember]
        public int ProcessId { get; set; }
        [DataMember]
        public PublicKey Key { get; set; }
    }
}