using System;
using Akka.Actor;

namespace AkkaStreams
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting AkkaStreams test program");

            using (ActorSystem sys = ActorSystem.Create("local-system"))
            {
                IActorRef receiver = sys.ActorOf(Props.Create(() => new ReceiverActor()));
                IActorRef sender = sys.ActorOf(Props.Create(() => new SenderActor(receiver)));

                Console.WriteLine("Start signal");
                sender.Tell(new Messages.StartSending());
            }

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
        }
    }
}
