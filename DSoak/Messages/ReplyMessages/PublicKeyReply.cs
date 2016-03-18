using System.Runtime.Serialization;
using SharedObjects;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class PublicKeyReply : Reply
    {
        static PublicKeyReply() { Register(typeof(PublicKeyReply)); }

        [DataMember]
        public int ProcessId { get; set; }
        [DataMember]
        public PublicKey Key { get; set; }
    }
}