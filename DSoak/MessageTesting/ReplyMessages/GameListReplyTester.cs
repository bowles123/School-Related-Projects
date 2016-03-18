using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;
using SharedObjects;

namespace MessageTesting.ReplyMessages
{
    [TestClass]
    public class GameListReplyTester
    {
        [TestMethod]
        public void GameListReply_TestEverything()
        {
            GameInfo g1 = new GameInfo()
            {
                GameId = 10,
                GameManagerId = 2,
                Label = "Test Game #1",
                MinPlayers = 3,
                MaxPlayers = 5,
                Status = GameInfo.StatusCode.Available
            };

            GameInfo g2 = new GameInfo()
            {
                GameId = 12,
                GameManagerId = 3,
                Label = "Test Game #2",
                MinPlayers = 3,
                MaxPlayers = 5,
                Status = GameInfo.StatusCode.Available
            };

            GameListReply r1 = new GameListReply() { Success = true, Note = "Test note", GameInfo = new [] { g1, g2 } };
            Assert.AreEqual(2, r1.GameInfo.Length);
            Assert.AreEqual(10, r1.GameInfo[0].GameId);
            Assert.AreEqual(12, r1.GameInfo[1].GameId);

            byte[] bytes = r1.Encode();
            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            GameListReply r2 = Message.Decode(bytes) as GameListReply;
            Assert.IsNotNull(r2);
            Assert.AreEqual(2, r2.GameInfo.Length);
            Assert.AreEqual(10, r2.GameInfo[0].GameId);
            Assert.AreEqual(12, r2.GameInfo[1].GameId);
        }
    }
}
