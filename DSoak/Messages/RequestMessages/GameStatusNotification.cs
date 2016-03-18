using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class GameStatusNotification : Request
    {
        [DataMember]
        public GameInfo Game { get; set; }
    }
}
