using SharedObjects;

namespace CommunicationSubsystem.Conversations.Responders
{
    public class ShutdownResponder: UnreliableMulticast
    {
        public event StateChange.Handler Shutdown;

        protected override void Process(object state)
        {
            logger.Debug("Executing shutdown responder.");

            Alive = false;
            if (MyProcess != null)
                MyProcess.Status = ProcessInfo.StatusCode.Terminating;
            RaiseShutdownEvent();
        }

        public void RaiseShutdownEvent()
        {
            logger.Debug("Enter RaiseShutdownEvent");
            if (Shutdown != null)
            {
                logger.Debug("Raise Shutdown event");
                Shutdown(new StateChange() { Type = StateChange.ChangeType.Shutdown, Subject = this });
                logger.Debug("Back from raising Shutdown event");
            }
            else
                logger.Debug("Nobody is registered for the shutdown");
            logger.Debug("Leave RaiseShutdownEvent");
        }
    }
}
