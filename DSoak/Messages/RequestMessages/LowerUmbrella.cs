using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class LowerUmbrella : Request
    {
        [DataMember]
        public int UmbrellaId { get; set; }
    }
}