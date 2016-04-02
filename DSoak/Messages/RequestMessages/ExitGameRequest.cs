using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class ExitGameRequest : Request
    {
        [DataMember]
        public int GameId { get; set; }
    }
}
