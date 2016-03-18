using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class LogoutRequestTester
    {
        [TestMethod]
        public void LogoutRequest_TestEverything()
        {
            LogoutRequest r1 = new LogoutRequest();
            Assert.IsNotNull(r1);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            LogoutRequest r2 = m2 as LogoutRequest;
            Assert.IsNotNull(r2);
        }
    }
}