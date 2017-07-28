using System;
using Akka.Actor;

namespace RemoteReceiver
{
    class ReceiverActor : ReceiveActor
    {
        public ReceiverActor()
        {
            ReceiveAny((message) => ReceivedAny(message));
        }

        private void ReceivedAny(object message)
        {
            Console.WriteLine($"[remote-receiver] got a {message.GetType().FullName}");
        }
    }
}
