using System;
using System.Runtime.Serialization;

using SharedObjects;

namespace Messages.RequestMessages
{
    [DataContract]
    public class LogoutRequest : Request
    {
    }
}
