using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using log4net;

namespace SharedObjects
{
    [DataContract]
    public class Penny : SharedResource
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Penny));
        private static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(Penny));

        public Penny Clone()
        {
            return MemberwiseClone() as Penny;
        }

        public override string ToString()
        {
            return string.Format("Penny #{0}", Id);
        }

        public byte[] Encode()
        {
            MemoryStream mstream = new MemoryStream();
            Serializer.WriteObject(mstream, this);
            return mstream.ToArray();
        }

        public static Penny Decode(byte[] bytes)
        {
            Penny result = null;
            if (bytes != null)
            {
                try
                {
                    MemoryStream mstream = new MemoryStream(bytes);
                    result = Serializer.ReadObject(mstream) as Penny;
                }
                catch (Exception err)
                {
                    Logger.WarnFormat("Cnanot decode a penny: {0}", err.Message);
                }
            }
            return result;      
        }

    }
}
