using System;
using Akka.Actor;

namespace AkkaStreams
{
    class ReceiverActor : ReceiveActor
    {
        public ReceiverActor()
        {
            ReceiveAny((message) => ReceivedAnyMessage(message));
        }

        public void ReceivedAnyMessage(object message)
        {
            Console.WriteLine($"[receiver] got message ${message.GetType().FullName}");
        }
    }
}
