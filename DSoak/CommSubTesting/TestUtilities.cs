using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommSub;

using log4net.Config;

namespace CommSubTesting
{
    [TestClass]
    public class TestUtilities
    {
        public static CommSubsystem SetupTestCommSubsystem(ConversationFactory factory)
        {
            CommSubsystem commSubsystem = new CommSubsystem()
            {
                MinPort = 12000,
                MaxPort = 12099,
                ConversationFactory = factory
            };
            commSubsystem.Initialize(null);
            commSubsystem.Start();

            return commSubsystem;
        }

        [AssemblyInitialize]
        public static void InitializeTesting(TestContext context)
        {
            XmlConfigurator.Configure();
        }
    }
}
