using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class DeadProcessNotification : Request
    {

        [DataMember]
        public int ProcessId { get; set; }
    }
}
