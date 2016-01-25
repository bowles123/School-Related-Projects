using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class LoginReply : Reply
    {
        [DataMember]
        public ProcessInfo ProcessInfo { get; set; }
    }
}
