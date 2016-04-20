using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class UmbrellaLoweredNotification : Request
    {
        [DataMember]
        public int UmbrellaId { get; set; }
    }
}