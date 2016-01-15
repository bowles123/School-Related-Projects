using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharedObjects;

namespace SharedObjectTesting
{
    [TestClass]
    public class ProcessInfoTester
    {
        [TestMethod]
        public void ProcessInfo_TestEverything()
        {
            ProcessInfo p1 = new ProcessInfo();
            Assert.AreEqual(0, p1.ProcessId);
            Assert.AreEqual(ProcessInfo.ProcessType.Unknown, p1.Type);
            Assert.IsNull(p1.EndPoint);
            Assert.IsNull(p1.Label);
            Assert.AreEqual(ProcessInfo.StatusCode.Unknown, p1.Status);
            Assert.IsNull(p1.AliveTimestamp);

            PublicEndPoint ep1 = new PublicEndPoint() { Host = "swcwin.serv.usu.edu", Port = 32541 };
            DateTime t1 = DateTime.Now;
            ProcessInfo p2 = new ProcessInfo()
                {
                    ProcessId = 10,
                    Type = ProcessInfo.ProcessType.Player,
                    EndPoint = ep1,
                    Label = "Test Process",
                    Status = ProcessInfo.StatusCode.Initializing,
                    AliveTimestamp = t1 
                };
            Assert.AreEqual(10, p2.ProcessId);
            Assert.AreEqual(ProcessInfo.ProcessType.Player, p2.Type);
            Assert.AreEqual(ep1, p2.EndPoint);
            Assert.AreEqual("Test Process", p2.Label);
            Assert.AreEqual(ProcessInfo.StatusCode.Initializing, p2.Status);
            Assert.AreEqual("Initializing", p2.StatusString);
            Assert.AreEqual(t1, p2.AliveTimestamp);
        }

    }
}
