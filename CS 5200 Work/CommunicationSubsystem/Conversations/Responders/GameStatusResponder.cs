using Messages.RequestMessages;
using SharedObjects;
using Messages;

namespace CommunicationSubsystem.Conversations.Responders
{
    public class GameStatusResponder: UnreliableMultiCastProxy
    {
        protected override void Process(object state)
        {
            logger.Debug("Executing GameStatusResponder.");
            Request = null;
            GameStatusNotification status = null;

            Request = Queue.Dequeue(Timeout);

            if (Request != null)
            {
                logger.Debug("Received a game status notification");
                Routing mssg = Request.Message as Routing;
                status = mssg.InnerMessage as GameStatusNotification;
                Game = status.Game;
                GameStatus = Game.Status;
                CurrentGameId = status.Game.GameId;
                CurrentProcesses = status.Game.CurrentProcesses;
                logger.DebugFormat("Current game status: {0}", GameStatus);

                foreach (GameProcessData process in CurrentProcesses)
                {
                    if (process.Type == ProcessInfo.ProcessType.BalloonStore)
                        BalloonStoreId = process.ProcessId;
                    else
                        if (process.Type == ProcessInfo.ProcessType.PennyBank)
                        PennyBankId = process.ProcessId;
                    else
                        if (process.Type == ProcessInfo.ProcessType.WaterServer)
                        WaterSourceId = process.ProcessId;
                }

                Dictionary.CloseQueue(Request.Message.ConvId);
            }
            Stop();
        }
    }
}
