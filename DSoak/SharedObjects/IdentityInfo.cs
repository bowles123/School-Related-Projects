using System;
using System.Text;
using System.Runtime.Serialization;

namespace SharedObjects
{
    [DataContract]
    public class IdentityInfo
    {
        [DataMember]
        public string ANumber { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Alias { get; set; }

        public IdentityInfo Clone()
        {
            return MemberwiseClone() as IdentityInfo;
        }
    }
}
