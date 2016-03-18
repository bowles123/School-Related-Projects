using System.Runtime.Serialization;

namespace Messages.RequestMessages
{
    [DataContract]
    public class NextIdRequest : Request
    {
        static NextIdRequest() { Register(typeof(NextIdRequest)); }

    }
}