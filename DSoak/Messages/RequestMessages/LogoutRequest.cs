using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class LogoutRequest : Request
    {
        static LogoutRequest() { Register(typeof(LogoutRequest)); }

    }
}