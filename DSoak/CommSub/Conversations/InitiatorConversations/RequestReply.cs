using System;

using Messages;
using Messages.ReplyMessages;

using log4net;
using SharedObjects;

namespace CommSub.Conversations.InitiatorConversations
{
    public abstract class RequestReply : Conversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RequestReply));

        /// <summary>
        /// When communicating directly with another process, set this property to the end point desired
        /// communication channel.  If communicating via proxy, then leave this null.
        /// </summary>
        public PublicEndPoint TargetEndPoint { get; set; }

        /// <summary>
        /// When communicating via proxy, set this property to the process id of the target process and
        /// leave the TargetEndPoint property blank.
        /// </summary>
        public int ToProcessId { get; set; }

        /// <summary>
        /// Executes this conversation synchronously.  Note that this is a template method, with three overridable parts:
        /// 
        ///     IsConversationStateValid()
        ///     IsProcessStateValid()
        ///     CreateRequest()
        ///     
        /// Also it uses ReliableSend, which is used ReplyHandler, and that another template method, with two override parts:
        /// 
        ///     ReplyProcessMustBeKnown
        ///     ProcessReply
        ///     PostFailureAction
        /// 
        /// </summary>
        /// <param name="context">A GameInfo object for the game to join</param>
        public override void Execute(object context = null)
        {
            Logger.DebugFormat("In Execute for {0}", GetType().Name);
            Done = false;

            if (IsConversationStateValid())
            {
                if (IsProcessStateValid())
                {
                    Logger.DebugFormat("Start {0} conversation", GetType().Name);
                    Message request = CreateRequest();
                    if (request != null)
                    {
                        request.InitMessageAndConversationNumbers();

                        Envelope env = new Envelope();
                        if (TargetEndPoint != null)
                        {
                            env.Message = request;
                            env.EndPoint = TargetEndPoint;
                        }
                        else
                        {
                            env.Message = new Routing()
                            {
                                InnerMessage = request,
                                ToProcessIds = new []{ ToProcessId }
                            };
                            env.EndPoint = Process.ProxyEndPoint;
                        }

                        ReliableSend(env);
                    }
                    else
                        Error = new Error() {Message = "No request message created"};
                }
                else
                    Error = Error.Get(Error.StandardErrorNumbers.InvalidProcessStateForConversation);
            }
            else
                Error = Error.Get(Error.StandardErrorNumbers.InvalidConversationSetup);

            if (Error != null)
            {
                Process.RecordError(Error);
                Logger.Warn(Error.Message);
            }

            Done = true;
            Logger.DebugFormat("End {0} Conversation", GetType().Name);
        }

        /// <summary>
        /// This method performs a reliable send using a timeout/retry loop
        /// </summary>
        /// <param name="envelope"></param>
        protected void ReliableSend(Envelope envelope)
        {
            QueueId = envelope.Message.MsgId;
            EnvelopeQueue queue = CommSubsystem.QueueDictionary.CreateQueue(QueueId);

            Logger.DebugFormat("In reliable send, with Queue={0}", QueueId);

            int retries = MaxRetries;

            while (retries > 0 && !Process.Quit)
            {
                Logger.DebugFormat("Send envelope with a {0} to {1}", envelope.Message.GetType().Name, envelope.IPEndPoint);
                if (MyCommunicator.Send(envelope))
                {
                    Logger.DebugFormat("Try to get a response");
                    Envelope replyEnvelope = queue.Dequeue(Timeout);
                    if (replyEnvelope != null)
                    {
                        Logger.DebugFormat("Got a reply {0} from {1}", replyEnvelope.Message.GetType().Name, replyEnvelope.IPEndPoint);
                        if (ReplyHandler(replyEnvelope))
                            break;
                        else
                            retries--;
                    }
                    else
                    {
                        Logger.DebugFormat("Nothing available in queue {0}", queue.QueueId);
                        retries--;
                    }
                }
                else
                {
                    Logger.Warn("Cannot send request");
                    retries = 0;
                }
            }
            if (retries == 0)
                PostFailureAction();

            CommSubsystem.QueueDictionary.CloseQueue(QueueId);
        }

        /// <summary>
        /// This method is the reply handler. It is a template method with two overridable parts:
        /// 
        ///     ProcessReply
        ///     PostFailureAction
        /// 
        /// </summary>
        /// <param name="replyEnvelope">The incoming reply message</param>
        /// <returns>True is the reply is valid</returns>
        protected bool ReplyHandler(Envelope replyEnvelope)
        {
            Logger.Debug("Enter ReplyHandler");
            bool result = false;
            if (IsEnvelopeValid(replyEnvelope, AllowedReplyTypes))
            {
                Reply reply = replyEnvelope.ActualMessage as Reply;               
                if (reply != null && reply.Success)
                {
                    Logger.DebugFormat("Received a {0} message back", reply.GetType().Name);
                    ProcessReply(reply);
                }
                else
                {
                    string errorMessage = string.Format("{0} failed because {1}", GetType().Name, (reply == null) ? string.Empty : reply.Note);
                    Process.RecordError(new Error() { Message = errorMessage });
                    Logger.Warn(errorMessage);
                    PostFailureAction();
                }
                result = true;
            }
            else
            {
                string errorMessage = string.Format("{0} is an invalid reply for a {1}", replyEnvelope.Message.GetType().Name, GetType().Name);
                Process.RecordError(new Error() { Message = errorMessage });
                Logger.Warn(errorMessage);
            }

            return result;
        }

        /// <summary>
        /// This method should return true when the conversation object contains the necessary information to execute.
        /// Override this method in concrete specializations, when necessary.
        /// </summary>
        /// <returns>true is the conversation contains all of the necessary information</returns>
        protected virtual bool IsConversationStateValid() { return (TargetEndPoint!=null || ToProcessId>0); }

        /// <summary>
        /// This method should return true when the process is in a valid state for this conversation to execute.
        /// Override this method in concrete specializations, when necessary.
        /// </summary>
        /// <returns>true if the process is in a state where it is okay for this conversation to execute</returns>
        protected virtual bool IsProcessStateValid() { return true; }

        /// <summary>
        /// This method should create the request message for the Request/Reply conversation.  This method must be
        /// overridden in the concrete specialization.
        /// </summary>
        /// <returns>The desired request message that will start the conversation</returns>
        protected abstract Message CreateRequest();

        /// <summary>
        /// This method returns an array of types of valid replys.  The concrete conversation must implement this method.
        /// </summary>
        protected abstract Type[] AllowedReplyTypes { get; }

        /// <summary>
        /// This method processes the reply.  The default behavior is a "do nothing" behavior.  A concrete conversation
        /// may override this method if the reply needs to change the state of the process or trigger another conversation.
        /// </summary>
        /// <param name="reply"></param>
        protected virtual void ProcessReply(Reply reply) { }

        /// <summary>
        /// This method execution some action on a failure conversation.  It will be called, for example, if the reliable
        /// send exhausted all possible retries.  A concrete conversation can override this methods to implement specific
        /// failure handling logic.
        /// </summary>
        protected virtual void PostFailureAction() { }
        
    }
}
