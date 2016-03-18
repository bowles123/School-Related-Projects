using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class GameListRequest : Request
    {
        static GameListRequest() { Register(typeof(GameListRequest)); }

        [DataMember]
        public int StatusFilter { get; set; }
    }
}
