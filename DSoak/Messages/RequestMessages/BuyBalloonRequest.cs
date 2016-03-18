using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class BuyBalloonRequest : Request
    {
        [DataMember]
        public Penny Penny { get; set; }
    }
}
