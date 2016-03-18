using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class ThrowBalloonRequest : Request
    {
        static ThrowBalloonRequest() { Register(typeof(ThrowBalloonRequest)); }

        [DataMember]
        public Balloon Balloon { get; set; }

        [DataMember]
        public int TargetPlayerId { get; set; }
    }
}
