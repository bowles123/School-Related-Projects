using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class GameListRequest : Request
    {
        [DataMember]
        public int StatusFilter { get; set; }
    }
}
