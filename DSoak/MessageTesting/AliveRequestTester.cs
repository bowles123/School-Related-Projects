using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;

namespace MessageTesting
{
    [TestClass]
    public class AliveRequestTester
    {
        [TestMethod]
        public void AliveRequest_TestEverything()
        {
            AliveRequest r1 = new AliveRequest();
            Assert.IsNotNull(r1);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);
 
            Message m2 = Message.Decode(bytes);
            AliveRequest r3 = m2 as AliveRequest;
            Assert.IsNotNull(r3);
        }
    }
}
