using System;
using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class RegisterGameRequest : Request
    {
        [DataMember]
        public GameInfo Game { get; set; }
    }
}
