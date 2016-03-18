using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class HitNotification : Request
    {
        static HitNotification() { Register(typeof(HitNotification)); }

        [DataMember]
        public int ByPlayerId { get; set; }
    }
}
