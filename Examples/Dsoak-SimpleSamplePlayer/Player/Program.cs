using System;
using System.Windows.Forms;

using log4net;
using log4net.Config;
using SharedObjects;

namespace Player
{
    static class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger((typeof (Program)));

        [STAThread]
        static void Main(string[] args)
        {
            // Setup up logging with log4net
            XmlConfigurator.Configure();
            Logger.Info("Simple Sample Player");

            // Create player object
            Player player = new Player()
            {
                RegistryEndPoint = new PublicEndPoint() {HostAndPort = Properties.Settings.Default.RegistryEndPoint},
                FirstName = Properties.Settings.Default.FirstName,
                LastName = Properties.Settings.Default.LastName,
                ANumber = Properties.Settings.Default.ANumber,
                Alias = Properties.Settings.Default.Alias,
                ProcessLabel = Properties.Settings.Default.ProcessName
            };

            player.Initialize();

            // Start the player
            player.Start();

            // Allow the user to stop by entering any keystroke
            Console.WriteLine("Press any key to stop player ...");
            Console.ReadKey(true);
            player.Stop();
        }
    }
}
