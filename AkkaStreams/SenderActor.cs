using System;
using System.IO;
using Akka.Actor;
using Akka.IO;
using Akka.Streams;
using Akka.Streams.Dsl;

namespace AkkaStreams
{
    class SenderActor : ReceiveActor
    {
        private readonly IActorRef receiver;
        private const string FILENAME = @"c:\ports.txt";
        private const int CHUNK_SIZE = 8192;

        public SenderActor(IActorRef receiver)
        {
            this.receiver = receiver;

            Receive<Messages.StartSending>((message) => ReceivedStartSending(message));
        }

        private void ReceivedStartSending(Messages.StartSending message)
        {
            var stream = new FileStream(FILENAME, FileMode.Open);

            var materializer = Context.Materializer();
            var source = StreamConverters.FromInputStream(() => stream, CHUNK_SIZE);
            var result = source.To(Sink.ActorRef<ByteString>(this.receiver, new Messages.StreamComplete()))
                .Run(materializer);
        }
    }
}
