using System;
using System.Net.Configuration;
using Utils;

using log4net;

namespace CommSub
{
    public class Dispatcher : BackgroundThread
    {
        #region Private Data Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Dispatcher));
        private const int TimeoutMs = 1000;
        #endregion

        public CommSubsystem CommSubsystem { get; set; }

        protected override void Process(Object state)
        {
            while (KeepGoing)
            {
                Envelope e = CommSubsystem.Communicator.Receive(TimeoutMs);
                if (e != null && e.Message!=null)
               {
                    if (e.Message.MsgId==null)
                        CommSubsystem.ParentProcess.ErrorHistory.Add(Error.Get(Error.StandardErrorNumbers.NullMessageNumber));
                    else if (e.Message.ConvId == null)
                        CommSubsystem.ParentProcess.ErrorHistory.Add(
                            Error.Get(Error.StandardErrorNumbers.NullConversationId));
                    else
                    {
                        Logger.DebugFormat("Received message: Type={0}, From Process={1}",
                            e.ActualMessage.GetType().Name,
                            e.ActualMessage.MsgId.Pid);
                        EnqueueEnvelope(e);
                    }
               }
            }
        }

        private void EnqueueEnvelope(Envelope e)
        {
            EnvelopeQueue queue = CommSubsystem.QueueDictionary.GetByConvId(e.Message.ConvId);

            if (queue != null)
            {
                Logger.DebugFormat("Placing message in the queue for conversation {0}", e.Message.ConvId);
                queue.Enqueue(e);
            }
            else
            {
                Type messageType = e.ActualMessageType;

                if (CommSubsystem.ConversationFactory.IncomingMessageCanStartConversation(messageType))
                    Dispatch(messageType, e);
                else
                    Logger.WarnFormat("Unexcepted incoming message of type {0}", messageType.Name);
            }
        }


        private void Dispatch(Type messageType, Envelope incomingRequestEnvelope)
        {
            Conversation conversation = CommSubsystem.ConversationFactory.CreateFromMessageType(messageType, incomingRequestEnvelope);
            if (conversation == null)
                Logger.WarnFormat("Cannot find strategy for {0}", messageType);
            else
            {
                Logger.DebugFormat("Dispatch request to strategy, type={0}, message from={1}", conversation.GetType().Name, incomingRequestEnvelope.IPEndPoint);
                conversation.Launch();
            }
        }
    }
}
