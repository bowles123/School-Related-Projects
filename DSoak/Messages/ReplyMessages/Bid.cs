using System.Runtime.Serialization;
using SharedObjects;

namespace Messages.ReplyMessages
{
    [DataContract]
    public class Bid : Reply
    {
        static Bid() { Register(typeof(Bid)); }

        [DataMember]
        public Penny[] Pennies { get; set; }
    }
}