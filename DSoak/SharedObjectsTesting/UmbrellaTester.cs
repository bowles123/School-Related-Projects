using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharedObjects;

namespace SharedObjectsTesting
{
    [TestClass]
    public class UmbrellaTester
    {
        [TestMethod]
        public void Umbrella_TestEverything()
        {
            Umbrella b1 = new Umbrella();
            Assert.AreEqual(0, b1.Id);
            Assert.IsNull(b1.DigitalSignature);

            Umbrella b2 = new Umbrella() { Id = 10 };
            Assert.AreEqual(10, b2.Id);
            Assert.IsNull(b2.DigitalSignature);
        }
    }
}
