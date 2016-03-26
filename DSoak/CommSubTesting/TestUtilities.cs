using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommSub;

using log4net.Config;

namespace CommSubTesting
{
    [TestClass]
    public class TestUtilities
    {
        public static DummyCommProcess SetupDummyCommProcess(int processId, Conversation.ActionHandler preAction, Conversation.ActionHandler postAction)
        {
            RuntimeOptions options = new DummyRuntimeOptions();
            options.SetDefaults();

            DummyCommProcess p = new DummyCommProcess()
            {
                AssignedProcessId = processId,
                Options = options
            };
            p.Start();
 
            p.CommSubsystem.ConversationFactory.PreExecuteAction = preAction;
            p.CommSubsystem.ConversationFactory.PostExecuteAction = postAction;

            return p;
        }

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
