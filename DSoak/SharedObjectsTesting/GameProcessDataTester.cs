using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharedObjects;

namespace SharedObjectsTesting
{
    [TestClass]
    public class GameProcessDataTester
    {
        [TestMethod]
        public void GameProcessData_TestEverything()
        {
            GameProcessData gpd1 = new GameProcessData();
            Assert.AreEqual(0, gpd1.ProcessId);
            Assert.AreEqual(ProcessInfo.ProcessType.Unknown, gpd1.Type);
            Assert.AreEqual(0, gpd1.LifePoints);
            Assert.AreEqual(0, gpd1.HitPoints);
            Assert.IsFalse(gpd1.HasUmbrellaRaised);

            GameProcessData gpd2 = new GameProcessData()
            {
                ProcessId = 10,
                Type = ProcessInfo.ProcessType.Player,
                LifePoints = 100,
                HitPoints = 20,
                HasUmbrellaRaised = true
            };

            Assert.AreEqual(10, gpd2.ProcessId);
            Assert.AreEqual(ProcessInfo.ProcessType.Player, gpd2.Type);
            Assert.AreEqual(100, gpd2.LifePoints);
            Assert.AreEqual(20, gpd2.HitPoints);
            Assert.IsTrue(gpd2.HasUmbrellaRaised);

            GameProcessData gpd3 = gpd2.Clone();
            Assert.AreEqual(gpd2.ProcessId, gpd3.ProcessId);
            Assert.AreEqual(gpd2.Type, gpd3.Type);
            Assert.AreEqual(gpd2.LifePoints, gpd3.LifePoints);
            Assert.AreEqual(gpd2.HitPoints, gpd3.HitPoints);
            Assert.AreEqual(gpd2.HasUmbrellaRaised, gpd3.HasUmbrellaRaised);

            DateTime ts1 = gpd3.LastChanged;
            Thread.Sleep(10);
            gpd3.LifePoints = 99;
            DateTime ts2 = gpd3.LastChanged;
            Assert.IsTrue(ts1 < ts2);

            ProcessInfo p1 = new ProcessInfo()
            {
                ProcessId = 21,
                Type = ProcessInfo.ProcessType.BalloonStore,
                Status = ProcessInfo.StatusCode.JoinedGame
            };

            GameProcessData gpd4 = new GameProcessData(p1);
            Assert.AreEqual(21, gpd4.ProcessId);
            Assert.AreEqual(ProcessInfo.ProcessType.BalloonStore, gpd4.Type);
            Assert.AreEqual(0, gpd4.LifePoints);
            Assert.AreEqual(0, gpd4.HitPoints);
            Assert.IsFalse(gpd4.HasUmbrellaRaised);

            // TODO: Test ChangeLifePoints
            // TODO: Test ChnageHitPoints
            // TODO: Test concurrent access
        }
    }
}
