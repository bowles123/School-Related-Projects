using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class DeadProcessNotification : Request
    {
        static DeadProcessNotification() { Register(typeof(DeadProcessNotification)); }

        [DataMember]
        public int ProcessId { get; set; }
    }
}
