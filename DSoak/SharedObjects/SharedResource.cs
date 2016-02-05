using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Security.Cryptography;

namespace SharedObjects
{
    [DataContract]
    public abstract class SharedResource
    {
        private static SHA1Managed hasher = new SHA1Managed();
        #region Public Methods
        [DataMember]
        public Int32 Id { get; set; }
        [DataMember]
        public byte[] DigitalSignature { get; set; }

        public byte[] ComputeHash()
        {
            byte[] hash = hasher.ComputeHash(Encoding.Unicode.GetBytes(this.ToString()));
            return hash;
        }

        #endregion
    }
}
