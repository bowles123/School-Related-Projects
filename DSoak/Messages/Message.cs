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
using Messages.ReplyMessages;
using Messages.RequestMessages;

namespace Messages
{
    [DataContract]
    public class Message
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Message));

        private static List<Type> serializableTypes = new List<Type>()
        {
            typeof(AliveRequest),
            typeof(GameListRequest),
            typeof(GameListReply),
            typeof(JoinGameReply),
            typeof(JoinGameRequest),
            typeof(LoginRequest),
            typeof(LoginReply),
            typeof(LogoutRequest),
            typeof(NextIdRequest),
            typeof(NextIdReply),
            typeof(RegisterGameReply),
            typeof(RegisterGameRequest),
            typeof(Reply),
            typeof(Request),
            typeof(ValidateProcessRequest)
        };

        public Message()
        {
        }

        public byte[] Encode()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Message), serializableTypes);
            MemoryStream mstream = new MemoryStream();
            serializer.WriteObject(mstream, this);

            return mstream.ToArray();
        }

        public static Message Decode(byte[] bytes)
        {
            Message result = null;
            if (bytes != null)
            {
                try
                {
                    MemoryStream mstream = new MemoryStream(bytes);
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Message), serializableTypes);
                    result = (Message)serializer.ReadObject(mstream);
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
