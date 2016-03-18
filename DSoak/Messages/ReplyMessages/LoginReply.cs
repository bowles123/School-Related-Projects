using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class LoginReply : Reply
    {
        static LoginReply() { Register(typeof(LoginReply)); }

        [DataMember]
        public ProcessInfo ProcessInfo { get; set; }

        [DataMember]
        public PublicEndPoint ProxyEndPoint { get; set; }

        [DataMember]
        public PublicEndPoint PennyBankEndPoint { get; set; }

        [DataMember]
        public PublicKey PennyBankPublicKey { get; set; }
    }
}
