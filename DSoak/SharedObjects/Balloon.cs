using System.Runtime.Serialization;

namespace SharedObjects
{
    [DataContract]
    public class Balloon : SharedResource
    {
        [DataMember]
        public bool IsFilled { get; set; }

        public override byte[] DataBytes()
        {
            byte[] baseBytes = base.DataBytes();
            byte[] myBytes = new byte[baseBytes.Length+1];
            baseBytes.CopyTo(myBytes, 0);
            myBytes[baseBytes.Length] = IsFilled ? (byte) 1 : (byte) 0;
            return myBytes;
        }

        public override string ToString()
        {
            return string.Format("Balloon #{0}, IsFilled={1}", Id, IsFilled);
        }
    }
}
