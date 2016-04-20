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
            RaiseUmbrellaRequest r1 = new RaiseUmbrellaRequest() { Umbrella  = new Umbrella() { Id = 10 }};
            Assert.IsNotNull(r1);
            Assert.AreEqual(10, r1.Umbrella.Id);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            RaiseUmbrellaRequest r2 = m2 as RaiseUmbrellaRequest;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.Umbrella.Id, r2.Umbrella.Id);
        }
    }
}