using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class RegisterGameRequest : Request
    {
        static RegisterGameRequest() { Register(typeof(RegisterGameRequest)); }

        [DataMember]
        public GameInfo Game { get; set; }
    }
}
