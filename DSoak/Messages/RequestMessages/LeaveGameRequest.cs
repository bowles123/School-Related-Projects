using System;
using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class LeaveGameRequest : Request
    {
        static LeaveGameRequest() { Register(typeof(LeaveGameRequest)); }
    }
}
