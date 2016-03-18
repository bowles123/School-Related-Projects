using System.Runtime.Serialization;

namespace SharedObjects
{
    [DataContract]
    public class Balloon : SharedResource
    {
        [DataMember]
        public bool IsFilled { get; set; }

        public override string ToString()
        {
            return string.Format("Balloon #{0}, IsFilled={1}", Id, IsFilled);
        }
    }
}
