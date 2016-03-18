using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class AllowanceAllocationRequest : Request
    {
        [DataMember]
        public int ToProcessId { get; set; }

        [DataMember]
        public int Amount { get; set; }
    }
}
