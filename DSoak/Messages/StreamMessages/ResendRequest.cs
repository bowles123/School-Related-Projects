using System;
using System.Runtime.Serialization;

namespace Messages.StreamMessages
{
    [DataContract]
    public class ResendRequest : StreamMessage
    {
        [DataMember]
        public int[] MissingMessages { get; set; }
    }
}
