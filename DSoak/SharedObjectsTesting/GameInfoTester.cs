using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharedObjects;

namespace SharedObjectTesting
{
    [TestClass]
    public class GameInfoTester
    {
        [TestMethod]
        public void GameInfo_TestEverything()
        {
            GameInfo g1 = new GameInfo();
            Assert.AreEqual(0, g1.GameId);
            Assert.IsNull(g1.Label);
            Assert.AreEqual(GameInfo.StatusCode.NotInitialized, g1.Status);
            Assert.AreEqual(2, g1.MinPlayers);
            Assert.AreEqual(20, g1.MaxPlayers);
            Assert.IsNull(g1.GameManager);
            Assert.AreEqual(0, g1.Winner);

            PublicEndPoint ep1 = new PublicEndPoint() { Host = "buzz.serv.usu.edu", Port = 20011 };
            PublicEndPoint ep2 = new PublicEndPoint() { Host = "buzz.serv.usu.edu", Port = 20012 };
            PublicEndPoint ep3 = new PublicEndPoint() { Host = "buzz.serv.usu.edu", Port = 20013 };

            ProcessInfo gm1 = new ProcessInfo()
                            {
                                ProcessId = 2,
                                Type= ProcessInfo.ProcessType.GameManager,
                                Label = "Game Manager 1",
                                EndPoint = ep1, Status = ProcessInfo.StatusCode.Registered,
                                AliveTimestamp = DateTime.Now 
                            };

            ProcessInfo player1 = new ProcessInfo()
            {
                ProcessId = 11,
                Type = ProcessInfo.ProcessType.Player,
                Label = "Player 1",
                EndPoint = ep2,
                Status = ProcessInfo.StatusCode.Registered,
                AliveTimestamp = DateTime.Now
            };

            ProcessInfo player2 = new ProcessInfo()
            {
                ProcessId = 12,
                Type = ProcessInfo.ProcessType.Player,
                Label = "Player 3",
                EndPoint = ep3,
                Status = ProcessInfo.StatusCode.Registered,
                AliveTimestamp = DateTime.Now
            };

            GameInfo g2 = new GameInfo()
                            {
                                GameId = 10,
                                Label="Test Game",
                                MinPlayers = 2,
                                MaxPlayers = 5,
                                Status = GameInfo.StatusCode.NotInitialized,
                                GameManager = gm1,
                                Winner = player1.ProcessId
                            };

            Assert.AreEqual(10, g2.GameId);
            Assert.AreEqual("Test Game", g2.Label);
            Assert.AreEqual(2, g2.MinPlayers);
            Assert.AreEqual(5, g2.MaxPlayers);
            Assert.AreEqual(GameInfo.StatusCode.NotInitialized, g2.Status);
            Assert.AreSame(gm1, g2.GameManager);
            Assert.AreEqual(player1.ProcessId, g2.Winner);

            GameInfo g3 = g2.Clone();
            Assert.IsNotNull(g3);
            Assert.AreNotSame(g2, g3);
            Assert.AreEqual(10, g3.GameId);
            Assert.AreEqual("Test Game", g3.Label);
            Assert.AreEqual(2, g3.MinPlayers);
            Assert.AreEqual(5, g3.MaxPlayers);
            Assert.AreEqual(GameInfo.StatusCode.NotInitialized, g3.Status);
            Assert.AreEqual(2, g3.GameManager.ProcessId);
            Assert.AreEqual(ep1, g3.GameManager.EndPoint);
            Assert.AreEqual(player1.ProcessId, g2.Winner);

            ProcessInfo p20 = new ProcessInfo() { ProcessId = 20, Label = "Test Process #20" };
            g3.AddCurrentProcess(p20);
            Assert.AreSame(p20, g3.FindCurrentProcess(20));

            ProcessInfo p21 = new ProcessInfo() { ProcessId = 21, Label = "Test Process #21" };
            g3.AddCurrentProcess(p21);
            Assert.AreSame(p21, g3.FindCurrentProcess(21));

            ProcessInfo p22 = new ProcessInfo() { ProcessId = 22, Label = "Test Process #22" };
            g3.AddCurrentProcess(p22);
            Assert.AreSame(p22, g3.FindCurrentProcess(22));

            g3.RemoveCurrentProcess(21);
            Assert.IsNull(g3.FindCurrentProcess(21));
        }

    }
}
