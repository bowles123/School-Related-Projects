using CommandLine;
using log4net.Config;
using System.Threading;

namespace UmbrellaSupplierProcess
{
    class MyUmbrellaSupplier
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            UmbrellaSupplier supplier = new UmbrellaSupplier();

            if (Parser.Default.ParseArguments(args, supplier.Options))
            {
                supplier.Options.SetDefaults();
                supplier.initialize();
                supplier.startUmbrellaSupplier();
                while (supplier.Status == "Running") Thread.Sleep(0);
            }
        }
    }
}
