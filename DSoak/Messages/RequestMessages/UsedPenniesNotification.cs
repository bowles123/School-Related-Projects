using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class UsedPenniesNotification : Request
    {
        [DataMember]
        public int[] PennyIds { get; set; }
    }
}
