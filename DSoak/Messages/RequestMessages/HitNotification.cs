using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class HitNotification : Request
    {
        [DataMember]
        public int ByPlayerId { get; set; }
    }
}
