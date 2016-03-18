using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class ReadyToStart : Request
    {
        static ReadyToStart() { Register(typeof(ReadyToStart)); }

        [DataMember]
        public int GameId { get; set; }
    }
}
