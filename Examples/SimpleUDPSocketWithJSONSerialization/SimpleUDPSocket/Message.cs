using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SimpleUDPSocket
{
    [DataContract]
    public class Message
    {
        private static DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(Message));

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
        [DataMember]
        public string Text { get; set; }

        public byte[] Encode()
        {
            MemoryStream mstrem = new MemoryStream();
            _serializer = new DataContractJsonSerializer(typeof(Message));
            _serializer.WriteObject(mstrem, this);
            byte[] bytes = mstrem.ToArray();

            return bytes;
        }

        public static Message Decode(byte[] bytes)
        {
            Message message = null;
            if (bytes != null)
            {
                MemoryStream mstream = new MemoryStream(bytes);
                message = _serializer.ReadObject(mstream) as Message;
            }

            return message;
        }
    }
}
