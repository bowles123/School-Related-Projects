using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class JoinGameRequest : Request
    {
        static JoinGameRequest() { Register(typeof(JoinGameRequest)); }

        [DataMember]
        public int GameId { get; set; }

        [DataMember]
        public ProcessInfo Player { get; set; }
    }
}
