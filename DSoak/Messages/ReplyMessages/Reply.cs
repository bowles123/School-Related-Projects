using System.Runtime.Serialization;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class Reply : Message
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Note { get; set; }
    }
}
