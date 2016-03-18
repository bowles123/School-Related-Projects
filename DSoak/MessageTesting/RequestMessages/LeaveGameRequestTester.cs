using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class LeaveGameRequestTester
    {
        [TestMethod]
        public void LeaveGameRequest_TestEverything()
        {
            LeaveGameRequest r1 = new LeaveGameRequest();
            Assert.IsNotNull(r1);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            LeaveGameRequest r2 = m2 as LeaveGameRequest;
            Assert.IsNotNull(r2);
        }
    }
}