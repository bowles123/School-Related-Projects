using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using log4net;

namespace SimpleUDPSocket
{
    [DataContract]
    public class Message
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (Message));
        private static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(Message));

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
        [DataMember]
        public string Text { get; set; }

        public byte[] Encode()
        {
            Logger.Debug("Encode message");

            MemoryStream mstrem = new MemoryStream();
            Serializer.WriteObject(mstrem, this);
            return mstrem.ToArray();
        }

        public static Message Decode(byte[] bytes)
        {
            Message message = null;
            if (bytes != null)
            {
                Logger.DebugFormat("Decode {0}", Encoding.ASCII.GetString(bytes));
                MemoryStream mstream = new MemoryStream(bytes);
                message = Serializer.ReadObject(mstream) as Message;
            }

            return message;
        }
    }
}
