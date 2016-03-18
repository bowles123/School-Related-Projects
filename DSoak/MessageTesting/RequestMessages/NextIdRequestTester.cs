using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class NextIdRequestTester
    {
        [TestMethod]
        public void NextIdRequest_TestEverything()
        {
            NextIdRequest r1 = new NextIdRequest();
            Assert.IsNotNull(r1);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            NextIdRequest r2 = m2 as NextIdRequest;
            Assert.IsNotNull(r2);
        }
    }
}