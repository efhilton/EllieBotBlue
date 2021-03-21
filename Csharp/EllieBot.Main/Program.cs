using EllieBot.NervousSystem;

namespace EllieBot.Main
{
    public class Program
    {
        public static  void Main(string[] args)
        {
            CommunicationBroker ns = new CommunicationBroker();
             ns.ConnectAsync().Wait();
             ns.PublishAsync(CommunicationBroker.COMMAND_TOPIC, "hello").Wait();
        }
    }
}
