using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class FillBalloonRequest : Request
    {
        [DataMember]
        public Balloon Balloon { get; set; }
        [DataMember]
        public Penny[] Pennies { get; set; }
    }
}
