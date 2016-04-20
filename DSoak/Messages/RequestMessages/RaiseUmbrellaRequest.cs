using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class RaiseUmbrellaRequest : Request
    {
        [DataMember]
        public Umbrella Umbrella { get; set; }
    }
}
