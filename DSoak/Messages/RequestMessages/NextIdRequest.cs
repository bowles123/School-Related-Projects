using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class NextIdRequest : Request
    {
        [DataMember]
        public int NumberOfIds { get; set; }
    }
}