using System.Runtime.Serialization;
using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class LoginRequest : Request
    {
        static LoginRequest() { Register(typeof(LoginRequest)); }

        [DataMember]
        public ProcessInfo.ProcessType ProcessType { get; set; }

        [DataMember]
        public string ProcessLabel { get; set; }

        [DataMember]
        public IdentityInfo Identity { get; set; }

        [DataMember]
        public PublicKey PublicKey { get; set; }
    }
}