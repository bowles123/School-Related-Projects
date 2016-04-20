using System;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace SharedObjects
{
    [DataContract]
    public abstract class SharedResource
    {

        #region Public Methods
        [DataMember]
        public Int32 Id { get; set; }

        [DataMember]
        public Int32 SignedBy { get; set; }

        [DataMember]
        public byte[] DigitalSignature { get; set; }

        public virtual byte[] DataBytes()
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Id));
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        #endregion
    }
}
