using CommunicationSubsystem;

namespace PlayerProcess
{
    public class PlayerOptions : RuntimeOptions
    {
        public override void SetDefaults()
        {
            if (string.IsNullOrWhiteSpace(ANumber))
                ANumber = "A00001234";
            if (string.IsNullOrWhiteSpace(FirstName))
                FirstName = "Susan";
            if (string.IsNullOrWhiteSpace(LastName))
                LastName = "Tester";
            if (string.IsNullOrWhiteSpace(Alias))
                Alias = "Brian's Player";
        }
    }
}
