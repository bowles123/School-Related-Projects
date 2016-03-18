using System.Runtime.Serialization;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class StartGame : Reply
    {
        static StartGame() { Register(typeof(StartGame)); }
    }
}
