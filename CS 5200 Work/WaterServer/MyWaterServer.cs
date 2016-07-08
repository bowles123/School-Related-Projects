using CommandLine;
using log4net.Config;
using System.Threading;

namespace WaterServerProcess
{
    class MyWaterServer
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            WaterServer server = new WaterServer();

            if (Parser.Default.ParseArguments(args, server.Options))
            {
                server.Options.SetDefaults();
                server.initialize();
                server.startWaterServer();
                while (server.Status == "Running") Thread.Sleep(0);
            }
        }
    }
}
