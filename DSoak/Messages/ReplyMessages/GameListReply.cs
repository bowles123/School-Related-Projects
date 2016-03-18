using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class GameListReply : Reply
    {
        static GameListReply() { Register(typeof(GameListReply)); }

        [DataMember]
        public GameInfo[] GameInfo { get; set; }
    }
}
