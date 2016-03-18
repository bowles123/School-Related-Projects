using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class BuyBalloonRequest : Request
    {
        static BuyBalloonRequest() { Register(typeof(BuyBalloonRequest)); }

        [DataMember]
        public Penny Penny { get; set; }
    }
}
