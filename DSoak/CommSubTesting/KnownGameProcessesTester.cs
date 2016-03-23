using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommSub;
using SharedObjects;

namespace CommSubTesting
{
    [TestClass]
    public class KnownGameProcessesTester
    {
        [TestMethod]
        public void KnownGameProcesses_TestEverything()
        {
            KnownGameProcesses knownProcesses = new KnownGameProcesses();

            List<GameProcessData> processList = knownProcesses.Processes;
            Assert.IsNotNull(processList);
            Assert.AreEqual(0, processList.Count);

            GameProcessData gpd1 = new GameProcessData()
            {
                ProcessId = 10,
                Type = ProcessInfo.ProcessType.BalloonStore
            };

            GameProcessData gpd2 = new GameProcessData()
            {
                ProcessId = 20,
                Type = ProcessInfo.ProcessType.GameManager
            };

            GameProcessData gpd3 = new GameProcessData()
            {
                ProcessId = 30,
                Type = ProcessInfo.ProcessType.Player
            };

            GameProcessData gpd4 = new GameProcessData()
            {
                ProcessId = 40,
                Type = ProcessInfo.ProcessType.Player
            };

            GameProcessData gpd5 = new GameProcessData()
            {
                ProcessId = 50,
                Type = ProcessInfo.ProcessType.Player
            };

            knownProcesses.AddOrUpdate(gpd1);
            processList = knownProcesses.Processes;
            Assert.IsNotNull(processList);
            Assert.AreEqual(1, processList.Count);
            Assert.AreSame(gpd1, processList[0]);
            Assert.AreSame(gpd1, knownProcesses[10]);

            knownProcesses.AddOrUpdate(gpd1);
            processList = knownProcesses.Processes;
            Assert.IsNotNull(processList);
            Assert.AreEqual(1, processList.Count);
            Assert.AreSame(gpd1, knownProcesses[10]);

            knownProcesses.AddOrUpdate(gpd2);
            processList = knownProcesses.Processes;
            Assert.IsNotNull(processList);
            Assert.AreEqual(2, processList.Count);
            Assert.AreSame(gpd1, knownProcesses[10]);
            Assert.AreSame(gpd2, knownProcesses[20]);

            knownProcesses.AddOrUpdate(gpd3);
            processList = knownProcesses.Processes;
            Assert.IsNotNull(processList);
            Assert.AreEqual(3, processList.Count);
            Assert.AreSame(gpd1, knownProcesses[10]);
            Assert.AreSame(gpd2, knownProcesses[20]);
            Assert.AreSame(gpd3, knownProcesses[30]);

            knownProcesses.AddOrUpdate(gpd4);
            processList = knownProcesses.Processes;
            Assert.IsNotNull(processList);
            Assert.AreEqual(4, processList.Count);
            Assert.AreSame(gpd1, knownProcesses[10]);
            Assert.AreSame(gpd2, knownProcesses[20]);
            Assert.AreSame(gpd3, knownProcesses[30]);
            Assert.AreSame(gpd4, knownProcesses[40]);

            knownProcesses.AddOrUpdate(gpd5);
            processList = knownProcesses.Processes;
            Assert.IsNotNull(processList);
            Assert.AreEqual(5, processList.Count);
            Assert.AreSame(gpd1, knownProcesses[10]);
            Assert.AreSame(gpd2, knownProcesses[20]);
            Assert.AreSame(gpd3, knownProcesses[30]);
            Assert.AreSame(gpd4, knownProcesses[40]);
            Assert.AreSame(gpd5, knownProcesses[50]);

            processList = knownProcesses.FilterProcesses(ProcessInfo.ProcessType.Player);
            Assert.AreEqual(3, processList.Count);
            Assert.IsTrue(processList.Contains(gpd3));
            Assert.IsTrue(processList.Contains(gpd4));
            Assert.IsTrue(processList.Contains(gpd5));

            knownProcesses.UpdateProcesses(new [] { gpd1, gpd3, gpd5});
            processList = knownProcesses.Processes;
            Assert.AreEqual(3, processList.Count);
            Assert.IsTrue(processList.Contains(gpd1));
            Assert.IsTrue(processList.Contains(gpd3));
            Assert.IsTrue(processList.Contains(gpd5));

            knownProcesses.UpdateProcesses(null);
            processList = knownProcesses.Processes;
            Assert.AreEqual(0, processList.Count);
        }
    }
}
