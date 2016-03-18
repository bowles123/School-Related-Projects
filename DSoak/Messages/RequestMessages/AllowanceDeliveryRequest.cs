using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class AllowanceDeliveryRequest : Request
    {
        static AllowanceDeliveryRequest() { Register(typeof(AllowanceDeliveryRequest)); }

        [DataMember]
        public int PortNumber { get; set; }

        [DataMember]
        public int NumberOfPennies { get; set; }
    }
}
