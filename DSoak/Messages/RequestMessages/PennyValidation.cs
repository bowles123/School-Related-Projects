using System.Runtime.Serialization;
using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class PennyValidation : Request
    {
        static PennyValidation() { Register(typeof(PennyValidation)); }

        [DataMember]
        public Penny[] Pennies { get; set; }

        [DataMember]
        public bool MarkAsUsedIfValid { get; set; }
    }
}
