using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.RequestMessages;
using  SharedObjects;

namespace MessageTesting.RequestMessages
{
    [TestClass]
    public class GameStatusNotificationTester
    {
        [TestMethod]
        public void GameStatusNotification_TestEverything()
        {
            GameInfo g1 = new GameInfo()
            {
                GameId = 10,
                GameManagerId = 11,
                Status = GameInfo.StatusCode.InProgress,
                MinPlayers = 2,
                MaxPlayers = 4,
                CurrentProcesses = new []
                {
                    new GameProcessData() {ProcessId = 12},
                    new GameProcessData() {ProcessId = 13}
                },
                Winners = null
            };
            GameStatusNotification r1 = new GameStatusNotification() { Game = g1 };
            Assert.IsNotNull(r1);
            Assert.AreSame(g1, r1.Game);

            byte[] bytes = r1.Encode();

            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            Message m2 = Message.Decode(bytes);
            GameStatusNotification r2 = m2 as GameStatusNotification;
            Assert.IsNotNull(r2);
            Assert.IsNotNull(r2.Game);
            Assert.AreEqual(r1.Game.GameId, r2.Game.GameId);
        }
    }
}
