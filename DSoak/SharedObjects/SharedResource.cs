using System;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace SharedObjects
{
    [DataContract]
    public abstract class SharedResource
    {
        private static readonly SHA1Managed Hasher = new SHA1Managed();

        #region Public Methods
        [DataMember]
        public Int32 Id { get; set; }

        [DataMember]
        public byte[] DigitalSignature { get; set; }

        public byte[] ComputeHash()
        {
            byte[] hash = Hasher.ComputeHash(Encoding.Unicode.GetBytes(ToString()));
            return hash;
        }

        public bool ValidateSignature()
        {
            // TODO: Add RSA parameter(s) and implement
            return true;
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        #endregion
    }
}
