using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class GameListReply : Reply
    {
        [DataMember]
        public GameInfo[] GameInfo { get; set; }
    }
}
