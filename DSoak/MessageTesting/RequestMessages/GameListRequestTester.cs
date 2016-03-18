using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class GameListRequestTester
    {
        [TestMethod]
        public void BuyBallooonRequest_TestEverything()
        {
            GameListRequest r1 = new GameListRequest() { StatusFilter = -1};
            Assert.IsNotNull(r1);
            Assert.AreEqual(-1, r1.StatusFilter);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            GameListRequest r2 = m2 as GameListRequest;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.StatusFilter, r2.StatusFilter);
        }
    }
}