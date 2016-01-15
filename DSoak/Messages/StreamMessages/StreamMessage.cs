using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using SharedObjects;
using log4net;

namespace Messages.StreamMessages
{
    [DataContract]
    public class StreamMessage
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Message));

        [DataMember]
        public int SeqNr { get; set; }

        private static List<Type> serializableTypes = new List<Type>()
        {
            typeof(InGame),
            typeof(NotInGame),
            typeof(StartGame),
            typeof(ResendRequest),
            typeof(KeyInfo)
        };

        public StreamMessage()
        {
        }

        public byte[] Encode()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(StreamMessage), serializableTypes);

            MemoryStream mstream = new MemoryStream();
            serializer.WriteObject(mstream, this);

            return mstream.ToArray();
        }

        public static StreamMessage Decode(byte[] bytes)
        {
            StreamMessage result = null;
            if (bytes != null)
            {
                try
                {
                    MemoryStream mstream = new MemoryStream(bytes);
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(StreamMessage), serializableTypes);
                    result = (StreamMessage)serializer.ReadObject(mstream);
                }
                catch (Exception err)
                {
                    _logger.WarnFormat("Except warning in decoding a message: {0}", err.Message);
                }
            }
            return result;
        }

    }
}
