using System.Runtime.Serialization;

namespace Messages
{
    [DataContract]
    public class Routing : Message
    {
        static Routing() { Register(typeof(Routing)); Register(typeof(Message)); }

        private Message _innerMessage;

        [DataMember]
        public Message InnerMessage
        {
            get { return _innerMessage; }
            set
            {
                _innerMessage = value;
                if (_innerMessage != null)
                {
                    MsgId = _innerMessage.MsgId;
                    ConvId = _innerMessage.ConvId;
                }
            }
        }

        [DataMember]
        public int[] ToProcessIds { get; set; }

        public int FromProcessId
        {
            get { return (MsgId == null) ? 0 : MsgId.Pid;  }
        }

    }
}
