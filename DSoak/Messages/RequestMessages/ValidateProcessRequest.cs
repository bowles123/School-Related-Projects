using System;
using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class ValidateProcessRequest : Request
    {
        [DataMember]
        public ProcessInfo Process { get; set; }
    }
}
