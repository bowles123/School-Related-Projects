using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class GetKeyRequest : Request
    {
        static GetKeyRequest() { Register(typeof(GetKeyRequest)); }

        [DataMember]
        public int ProcessId { get; set; }
    }
}
