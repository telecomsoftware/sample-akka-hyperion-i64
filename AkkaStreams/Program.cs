using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaStreams
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ActorSystem sys = ActorSystem.Create("local-system"))
            {
                IActorRef receiver = sys.ActorOf(Props.Create(() => new ReceiverActor()));
                IActorRef sender = sys.ActorOf(Props.Create(() => new SenderActor(receiver)));

                sender.Tell(new Messages.StartSending());
            }

            Console.ReadLine();
        }
    }
}
