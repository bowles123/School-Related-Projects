using System.Runtime.Serialization;
using SharedObjects;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class Bid : Reply
    {
        [DataMember]
        public Penny[] Pennies { get; set; }
    }
}