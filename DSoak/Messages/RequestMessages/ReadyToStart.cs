using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class ReadyToStart : Request
    {
        [DataMember]
        public int GameId { get; set; }
    }
}
