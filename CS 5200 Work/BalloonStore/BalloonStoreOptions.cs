using CommunicationSubsystem;
using CommandLine;

namespace BalloonStoreProcess
{
    public class BalloonStoreOptions : RuntimeOptions
    {
        [Option("gmid", MetaValue = "INT", Required = true, HelpText = "Game manager process id")]
        public int GameManagerId { get; set; }

        [Option("gameid", MetaValue = "INT", Required = true, HelpText = "Game id")]
        public int GameId { get; set; }

        [Option("balloons", MetaValue = "INT", Required = true, HelpText = "Number of balloons for the balloon store")]
        public int NumBalloons { get; set; }

        [Option("storeindex", MetaValue = "INT", Required = true, HelpText = "Index of the balloon store")]
        public int StoreIndex { get; set; }

        [Option("gmep", MetaValue = "STRING", Required = false, HelpText = "Game Manager Endpoint")]
        public string GameManagerEndpoint { get; set; }

        public override void SetDefaults()
        {
            if (string.IsNullOrWhiteSpace(ANumber))
                ANumber = "A00001234";
            if (string.IsNullOrWhiteSpace(FirstName))
                FirstName = "Susan";
            if (string.IsNullOrWhiteSpace(LastName))
                LastName = "Tester";
            if (string.IsNullOrWhiteSpace(Alias))
                Alias = "Brian's Balloon Store";
        }
    }
}
