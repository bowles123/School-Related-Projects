using System;

namespace TcpInitiator
{
    public class Program
    {
        static void Main(string[] args)
        {
            string message = (args.Length > 0) ? args[0] : "Nothing to says";
            int repeatCount = 1;
            if (args.Length > 1)
                Int32.TryParse(args[1], out repeatCount);

            Sender sender = new Sender() { MessageToSend = message, RepeatCount = repeatCount};
            sender.Run();

        }
    }
}
