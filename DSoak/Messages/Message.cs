using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;
using log4net;

namespace Messages
{
    [DataContract]
    public class Message
    {
        static Message()
        {
            Register(typeof(BalloonReply));
            Register(typeof(Bid));
            Register(typeof(BidAck));
            Register(typeof(GameListReply));
            Register(typeof(JoinGameReply));
            Register(typeof(LoginReply));
            Register(typeof(NextIdReply));
            Register(typeof(PublicKeyReply));
            Register(typeof(Reply));
            Register(typeof(StartGame));

            Register(typeof (AliveRequest));
            Register(typeof(AllowanceAllocationRequest));
            Register(typeof(AllowanceDeliveryRequest));
            Register(typeof(AuctionAnnouncement));
            Register(typeof(BuyBalloonRequest));
            Register(typeof(DeadProcessNotification));
            Register(typeof(FillBalloonRequest));
            Register(typeof(GameListRequest));
            Register(typeof(GameStatusNotification));
            Register(typeof(GetKeyRequest));
            Register(typeof(HitNotification));
            Register(typeof(JoinGameRequest));
            Register(typeof(LeaveGameRequest));
            Register(typeof(LoginRequest));
            Register(typeof(LogoutRequest));
            Register(typeof(LowerUmbrella));
            Register(typeof(NextIdRequest));
            Register(typeof(PennyValidation));
            Register(typeof(RaiseUmbrella));
            Register(typeof(ReadyToStart));
            Register(typeof(RegisterGameRequest));
            Register(typeof(ShutdownRequest));
            Register(typeof(ThrowBalloonRequest));
        }

        private static readonly ILog Logger = LogManager.GetLogger(typeof(Message));

        private static List<Type> _serializableTypes;

        public static void Register(Type T)
        {
            if (_serializableTypes==null)
                _serializableTypes = new List<Type>();

            if (!_serializableTypes.Contains(T))
                _serializableTypes.Add(T);
        }

        [DataMember]
        public MessageNumber MsgId { get; set; }
        [DataMember]
        public MessageNumber ConvId { get; set; }

        /// <summary>
        /// This method sets up the message and conversation numbers for the first message of a conversation.
        /// Specifically, it creates a new message number, then that is copied for the conversation id.
        /// </summary>
        public void InitMessageAndConversationNumbers()
        {
            SetMessageAndConversationNumbers(MessageNumber.Create());
        }

        /// <summary>
        /// This method sets up the message and conversation id's for the first message of a conversation.
        /// Specifically, it sets the messsage id to the provided id, then that is copied for the conversation id.
        /// </summary>
        /// <param name="id">Message number that will become this message's message id and conversation id </param>
        public void SetMessageAndConversationNumbers(MessageNumber id)
        {
            SetMessageAndConversationNumbers(id, id.Clone());
        }

        /// <summary>
        /// This method set the message and conversations id's
        /// </summary>
        /// <param name="id"></param>
        /// <param name="convId"></param>
        public void SetMessageAndConversationNumbers(MessageNumber id, MessageNumber convId)
        {
            MsgId = id;
            ConvId = convId;
        }

        /// <summary>
        /// This method encodes a message into a byte array by first serializaing it into a JSON string and then
        /// converting that string to byte. 
        /// </summary>
        /// <returns>A byre array containing the JSON serializations of the message</returns>
        public byte[] Encode()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Message), _serializableTypes);
            MemoryStream mstream = new MemoryStream();
            serializer.WriteObject(mstream, this);

            return mstream.ToArray();
        }

        /// <summary>
        /// This is a simpel factory methods that tries to create a message objects from a byte arrary contains
        /// a JSON serializations of the message.
        /// </summary>
        /// <param name="bytes">A byte array containing an ASCII encoding of a correct JSON serialization of a valid message.</param>
        /// <returns>If successful, a message object, instantied as an instance of the correct specialization of Message.
        /// If unsucessful because the byte array did not contain a correctly serialized message, then null.</returns>
        public static Message Decode(byte[] bytes)
        {
            Message result = null;
            if (bytes != null)
            {
                try
                {
                    MemoryStream mstream = new MemoryStream(bytes);
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Message), _serializableTypes);
                    result = (Message)serializer.ReadObject(mstream);
                }
                catch (Exception err)
                {
                    Logger.WarnFormat("Except warning in decoding a message: {0}", err.Message);
                }
            }
            return result;
        }

    }
}
