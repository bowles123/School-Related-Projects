using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class ShutdownRequest : Request
    {
        static ShutdownRequest() { Register(typeof(ShutdownRequest)); }

    }
}
