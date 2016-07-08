using CommunicationSubsystem;
using CommandLine;

namespace UmbrellaSupplierProcess
{
    public class UmbrellaSupplierOptions : RuntimeOptions
    {
        [Option("gmid", MetaValue = "INT", Required = true, HelpText = "Game manager process id")]
        public int GameManagerId { get; set; }

        [Option("gameid", MetaValue = "INT", Required = true, HelpText = "Game id")]
        public int GameId { get; set; }

        [Option("umbrellas", MetaValue = "INT", Required = true, HelpText = "Number of umbrellas for the umbrella supplier")]
        public int NumUmbrellas { get; set; }

        [Option("supplierindex", MetaValue = "INT", Required = true, HelpText = "Index of the umbrellla supplier")]
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
