using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class RaiseUmbrellaTester
    {
        [TestMethod]
        public void RaiseUmbrella_TestEverything()
        {
            RaiseUmbrella r1 = new RaiseUmbrella() { Umbrella  = new Umbrella() { Id = 10 }};
            Assert.IsNotNull(r1);
            Assert.AreEqual(10, r1.Umbrella.Id);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            RaiseUmbrella r2 = m2 as RaiseUmbrella;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.Umbrella.Id, r2.Umbrella.Id);
        }
    }
}