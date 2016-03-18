using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class RaiseUmbrella : Request
    {
        [DataMember]
        public Umbrella Umbrella { get; set; }
    }
}
