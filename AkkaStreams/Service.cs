using System;
using Akka.Actor;
using SharedMessages;
using Topshelf;

namespace AkkaStreams
{
    internal class Service
    {
        private ActorSystem system;
        private HostControl hostControl;

        public bool Start(HostControl hostControl)
        {
            Console.WriteLine("Starting");
            this.hostControl = hostControl;

            this.system = ActorSystem.Create("local-system");

            IActorRef receiver = this.system.ActorOf(Props.Create(() => new ReceiverActor()));
            IActorRef sender = this.system.ActorOf(Props.Create(() => new SenderActor(receiver)));

            sender.Tell(new Messages.StartSending());

            Console.WriteLine("Started");
            return true;
        }

        public void Stop()
        {
            Console.WriteLine("Stopping");

            this.system.Terminate().Wait();

            Console.WriteLine("Stopped");
        }
    }
}
