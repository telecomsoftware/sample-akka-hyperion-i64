﻿using System;
using System.IO;
using System.Text;
using Akka.Actor;
using Akka.IO;
using SharedMessages;

namespace AkkaStreams
{
    class ReceiverActor : ReceiveActor
    {
        private MemoryStream stream;

        public ReceiverActor()
        {
            this.stream = new MemoryStream();

            Receive<ByteString>((message) => ReceivedStreamChunk(message));
            Receive<byte[]>((message) => ReceivedByteChunk(message));
            Receive<Messages.StreamComplete>((message) => ReceivedStreamComplete(message));
            Receive<Terminated>((message) => ReceivedTerminated(message));
            ReceiveAny((message) => ReceivedAnyMessage(message));
        }

        protected override void PostStop()
        {
            this.stream.Dispose();
        }

        private void ReceivedTerminated(Terminated message)
        {
            Console.WriteLine($"[receiver] {message.ActorRef.Path.ToStringWithoutAddress()} terminated, local stream length {(this.stream.CanRead ? this.stream.Length : -1)}");
        }

        private void ReceivedStreamComplete(Messages.StreamComplete message)
        {
            Console.WriteLine($"[receiver] got signaled that the stream completed, local stream length {this.stream.Length}");
        }

        private void ReceivedStreamChunk(ByteString message)
        {
            var bytes = message.ToArray();
            this.stream.Write(bytes, 0, bytes.Length);
            Console.WriteLine($"[receiver] got chunk, now stream length {this.stream.Length}");
        }

        private void ReceivedByteChunk(byte[] message)
        {
            this.stream.Write(message, 0, message.Length);
            Console.WriteLine($"[receiver] got raw-chunk, now stream length {this.stream.Length}");
        }

        private void ReceivedAnyMessage(object message)
        {
            Console.WriteLine($"[receiver] got message {message.GetType().FullName}");
        }
    }
}
