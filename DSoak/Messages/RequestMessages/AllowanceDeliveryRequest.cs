using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class AllowanceDeliveryRequest : Request
    {

        [DataMember]
        public int PortNumber { get; set; }

        [DataMember]
        public int NumberOfPennies { get; set; }
    }
}
