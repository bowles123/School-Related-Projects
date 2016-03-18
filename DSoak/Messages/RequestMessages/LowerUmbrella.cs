using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class LowerUmbrella : Request
    {
        static LowerUmbrella() { Register(typeof(LowerUmbrella)); }

        [DataMember]
        public int UmbrellaId { get; set; }
    }
}