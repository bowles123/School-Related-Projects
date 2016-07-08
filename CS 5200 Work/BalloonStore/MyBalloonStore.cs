using CommandLine;
using log4net.Config;
using System.Threading;

namespace BalloonStoreProcess
{
    class MyBalloonStore
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            BalloonStore store = new BalloonStore();

            if (Parser.Default.ParseArguments(args, store.Options))
            {
                store.Options.SetDefaults();
                store.initialize();
                store.startBalloonStore();
                while (store.Status == "Running") Thread.Sleep(0);
            }
        }
    }
}
