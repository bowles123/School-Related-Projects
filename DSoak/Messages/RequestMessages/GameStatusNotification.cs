using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class GameStatusNotification : Request
    {
        static GameStatusNotification() { Register(typeof(GameStatusNotification)); }

        [DataMember]
        public GameInfo Game { get; set; }
    }
}
