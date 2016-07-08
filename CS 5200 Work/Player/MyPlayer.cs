using log4net.Config;
using System.Threading;
using CommandLine;

namespace PlayerProcess
{
    class MyPlayer
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            Player player = new Player();

            if (Parser.Default.ParseArguments(args, player.Options))
            {
                player.Options.SetDefaults();
                player.initialize();
                player.startPlayer();
                while (player.Status == "Running") Thread.Sleep(0);
            }
        }
    }
}
