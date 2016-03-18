using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharedObjects;

namespace SharedObjectsTesting
{
   [TestClass]
    public class BalloonTester
    {
       [TestMethod]
       public void Balloon_TestEverything()
       {
           Balloon b1 = new Balloon();
           Assert.AreEqual(0, b1.Id);
           Assert.IsFalse(b1.IsFilled);
           Assert.IsNull(b1.DigitalSignature);

           Balloon b2 = new Balloon() {Id = 10, IsFilled = true};
           Assert.AreEqual(10, b2.Id);
           Assert.IsTrue(b2.IsFilled);
           Assert.IsNull(b2.DigitalSignature);
       }
    }
}
