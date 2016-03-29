using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class JoinGameRequest : Request
    {
        [DataMember]
        public int GameId { get; set; }

        [DataMember]
        public ProcessInfo Process { get; set; }
    }
}
