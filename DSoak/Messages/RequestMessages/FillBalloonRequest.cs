using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class FillBalloonRequest : Request
    {
        static FillBalloonRequest() { Register(typeof(FillBalloonRequest)); }

        [DataMember]
        public Balloon Balloon { get; set; }
        [DataMember]
        public Penny[] Pennies { get; set; }
    }
}
