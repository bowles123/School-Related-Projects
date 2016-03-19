using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharedObjects;


namespace SharedObjectsTesting
{
    [TestClass]
    public class GameInfoTester
    {
        [TestMethod]
        public void GameInfo_TestEverything()
        {
            GameInfo g1 = new GameInfo();
            Assert.AreEqual(0, g1.GameId);
            Assert.AreEqual(0,g1.GameManagerId);
            Assert.IsNull(g1.Label);
            Assert.AreEqual(GameInfo.StatusCode.NotInitialized, g1.Status);
            Assert.AreEqual(2, g1.MinPlayers);
            Assert.AreEqual(20, g1.MaxPlayers);
            Assert.IsNotNull(g1.StartingPlayers);
            Assert.AreEqual(0, g1.StartingPlayers.Length);
            Assert.IsNotNull(g1.CurrentProcesses);
            Assert.AreEqual(0, g1.CurrentProcesses.Length);
            Assert.IsNull(g1.Winners);

            GameInfo g2 = new GameInfo()
                            {
                                GameId = 10,
                                GameManagerId = 11,
                                Label="Test Game",
                                Status = GameInfo.StatusCode.Complete,
                                MinPlayers = 2,
                                MaxPlayers = 5,
                                StartingPlayers = new [] { 12, 13, 14, 15 },
                                CurrentProcesses = new []
                                {
                                    new GameProcessData() { ProcessId = 12, Type = ProcessInfo.ProcessType.Player},
                                    new GameProcessData() { ProcessId = 13, Type = ProcessInfo.ProcessType.Player },
                                    new GameProcessData() { ProcessId = 14, Type = ProcessInfo.ProcessType.Player },
                                    new GameProcessData() { ProcessId = 15, Type = ProcessInfo.ProcessType.Player },
                                    new GameProcessData() { ProcessId = 16, Type = ProcessInfo.ProcessType.BalloonStore },
                                    new GameProcessData() { ProcessId = 17, Type = ProcessInfo.ProcessType.WaterServer },
                                    new GameProcessData() { ProcessId = 18, Type = ProcessInfo.ProcessType.UmbrellaSupplier }
                                },
                                Winners = new [] { 12 }
                            };

            Assert.AreEqual(10, g2.GameId);
            Assert.AreEqual(11, g2.GameManagerId);
            Assert.AreEqual(GameInfo.StatusCode.Complete, g2.Status);
            Assert.AreEqual("Test Game", g2.Label);
            Assert.AreEqual(2, g2.MinPlayers);
            Assert.AreEqual(5, g2.MaxPlayers);
            Assert.AreEqual(4, g2.StartingPlayers.Length);
            Assert.AreEqual(7, g2.CurrentProcesses.Length);
            for (int i=0; i<g2.CurrentProcesses.Length; i++)
                Assert.AreEqual(i+12, g2.CurrentProcesses[i].ProcessId);
            Assert.AreEqual(1, g2.Winners.Length);
            Assert.AreEqual(12, g2.Winners[0]);

            GameInfo g3 = g2.Clone();
            Assert.AreEqual(10, g3.GameId);
            Assert.AreEqual(11, g3.GameManagerId);
            Assert.AreEqual(GameInfo.StatusCode.Complete, g3.Status);
            Assert.AreEqual("Test Game", g3.Label);
            Assert.AreEqual(2, g3.MinPlayers);
            Assert.AreEqual(5, g3.MaxPlayers);
            Assert.AreEqual(4, g3.StartingPlayers.Length);
            Assert.AreEqual(7, g3.CurrentProcesses.Length);
            for (int i = 0; i < g3.CurrentProcesses.Length; i++)
                Assert.AreEqual(i + 12, g3.CurrentProcesses[i].ProcessId);
            Assert.AreEqual(1, g3.Winners.Length);
            Assert.AreEqual(12, g3.Winners[0]);

        }

    }
}
