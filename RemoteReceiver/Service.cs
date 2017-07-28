using Akka.Actor;
using SharedMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace RemoteReceiver
{
    internal class Service
    {
        private ActorSystem system;
        private HostControl hostControl;

        public bool Start(HostControl hostControl)
        {
            Console.WriteLine("Starting");
            this.hostControl = hostControl;

            this.system = ActorSystem.Create("another-local-system");

            IActorRef receiver = this.system.ActorOf(Props.Create(() => new ReceiverActor()), "receiver-one");

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
