using Messages.RequestMessages;

namespace CommSubTesting
{
    public class DummyRequest : Request
    {
        public DummyRequest()
        {
            Register(typeof(DummyRequest));
        }
    }
}
