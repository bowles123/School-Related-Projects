using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SimpleUDPSocket
{
    [DataContract]
    public class Message
    {
        private static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(Message));

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
        [DataMember]
        public string Text { get; set; }

        public byte[] Encode()
        {
            MemoryStream mstrem = new MemoryStream();
            Serializer.WriteObject(mstrem, this);
            return mstrem.ToArray();
        }

        public static Message Decode(byte[] bytes)
        {
            Message message = null;
            if (bytes != null)
            {
                MemoryStream mstream = new MemoryStream(bytes);
                message = Serializer.ReadObject(mstream) as Message;
            }

            return message;
        }
    }
}
