using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class ShutdownRequestTester
    {
        [TestMethod]
        public void RegisterGameRequest_TestEverything()
        {
            ShutdownRequest r1 = new ShutdownRequest();
            Assert.IsNotNull(r1);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            ShutdownRequest r2 = m2 as ShutdownRequest;
            Assert.IsNotNull(r2);
        }
    }
}