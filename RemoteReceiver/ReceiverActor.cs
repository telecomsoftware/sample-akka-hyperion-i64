using System;
using Akka.Actor;
using System.IO;
using SharedMessages;

namespace RemoteReceiver
{
    class ReceiverActor : ReceiveActor
    {
        private MemoryStream stream;

        public ReceiverActor()
        {
            this.stream = new MemoryStream();
            Receive<byte[]>((message) => ReceivedByteChunk(message));
            Receive<Messages.StreamComplete>((message) => ReceivedStreamComplete(message));
            ReceiveAny((message) => ReceivedAny(message));
        }

        private void ReceivedByteChunk(byte[] message)
        {
            this.stream.Write(message, 0, message.Length);
            Console.WriteLine($"[remote-receiver] got raw-chunk, now stream length {this.stream.Length}");
        }

        private void ReceivedStreamComplete(Messages.StreamComplete message)
        {
            Console.WriteLine($"[remote-receiver] got signaled that the stream completed, local stream length {this.stream.Length}");
        }

        private void ReceivedAny(object message)
        {
            Console.WriteLine($"[remote-receiver] got a {message.GetType().FullName}");
        }
    }
}
