using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class RaiseUmbrella : Request
    {
        static RaiseUmbrella() { Register(typeof(RaiseUmbrella)); }

        [DataMember]
        public Umbrella Umbrella { get; set; }
    }
}
