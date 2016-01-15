using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messages;
using Messages.ReplyMessages;
using SharedObjects;

namespace MessageTesting.ReplyTesters
{
    [TestClass]
    public class GameListReplyTester
    {
        [TestMethod]
        public void GameListReply_TestEverything()
        {
            ProcessInfo gm1 = new ProcessInfo()
                {
                    ProcessId=1,
                    Label="Game Manager 1",
                    EndPoint = new PublicEndPoint() { Host = "buzz.serv.usu.edu", Port = 20011 }
                };
            ProcessInfo gm2 = new ProcessInfo()
                {
                    ProcessId=2,
                    Label="Game Manager 2",
                    EndPoint = new PublicEndPoint() { Host = "buzz.serv.usu.edu", Port = 20022 }
                };

            GameInfo g1 = new GameInfo()
            {
                GameManager = gm1,
                GameId = 10,
                Label = "Test Game #1",
                MinPlayers = 3,
                MaxPlayers = 5,
                Status = GameInfo.StatusCode.Available
            };

            GameInfo g2 = new GameInfo()
            {
                GameManager = gm2,
                GameId = 12,
                Label = "Test Game #2",
                MinPlayers = 3,
                MaxPlayers = 5,
                Status = GameInfo.StatusCode.Available
            };

            GameListReply r1 = new GameListReply() { Success = true, Note = "Test note", GameInfo = new GameInfo[] { g1, g2 } };

            byte[] bytes = r1.Encode();
            string tmp = ASCIIEncoding.ASCII.GetString(bytes);

            GameListReply r2 = Message.Decode(bytes) as GameListReply;
            Assert.IsNotNull(r2);
        }
    }
}
