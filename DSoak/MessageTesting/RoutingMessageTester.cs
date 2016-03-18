using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;

namespace MessageTesting
{
    [TestClass]
    public class RoutingMessageTester
    {
        [TestMethod]
        public void Routing_TestEverything()
        {
            AliveRequest r1 = new AliveRequest();
            Assert.IsNotNull(r1);

            Routing r2 = new Routing()
            {
                InnerMessage = r1,
                ToProcessIds = new[] {12, 13, 14}
            };

            byte[] bytes = r2.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            Assert.IsNotNull(m2);
            Routing r3 = m2 as Routing;
            Assert.IsNotNull(r3);
            Assert.IsNotNull(r3.InnerMessage);
            AliveRequest r4 = r3.InnerMessage as AliveRequest;
            Assert.IsNotNull(r4);

        }
    }
}
