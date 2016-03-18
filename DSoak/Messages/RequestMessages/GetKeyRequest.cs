using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class GetKeyRequest : Request
    {
        [DataMember]
        public int ProcessId { get; set; }
    }
}
