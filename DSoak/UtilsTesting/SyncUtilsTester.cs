using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Utils;

namespace UtilsTesting
{
    [TestClass]
    public class SyncUtilsTester
    {
        private bool _flag;

        [TestMethod]
        public void SyncUtils_TestEverything()
        {
            // Case 0 - Timeout will occur
            DateTime ts = DateTime.Now;
            bool result = SyncUtils.WaitForCondition(() => _flag == true, 4000);
            Assert.IsFalse(DateTime.Now.Subtract(ts).TotalMilliseconds < 4000);
            Assert.IsFalse(result);


            // Case 1 - Condition will be met before timeout
            Timer timer = new Timer(SetFlag, null, 1000, 1000);

            ts = DateTime.Now;
            result = SyncUtils.WaitForCondition(() => _flag==true, 4000);
            Assert.IsTrue(DateTime.Now.Subtract(ts).TotalMilliseconds<4000);
            Assert.IsTrue(result);
        }

        private void SetFlag(object state)
        {
            _flag = true;
        }

    }
}
