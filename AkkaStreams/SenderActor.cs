using System;
using System.IO;
using Akka.Actor;
using Akka.IO;
using Akka.Streams;
using Akka.Streams.Dsl;
using SharedMessages;

namespace AkkaStreams
{
    class SenderActor : ReceiveActor
    {
        private readonly IActorRef receiver;
        private const string FILENAME = @"d:\ports.txt";
        private const int CHUNK_SIZE = 8192;

        public SenderActor(IActorRef receiver)
        {
            this.receiver = receiver;

            Receive<Messages.StartSending>((message) => ReceivedStartSending(message));
        }

        private void ReceivedStartSending(Messages.StartSending message)
        {
            Console.WriteLine($"[sender] ReceivedStartSending()");
            var stream = new FileStream(FILENAME, FileMode.Open);

            var materializer = Context.Materializer();
            var source = StreamConverters.FromInputStream(() => stream, CHUNK_SIZE);
            var result = source.To(Sink.ActorRef<ByteString>(this.receiver, new Messages.StreamComplete()))
                .Run(materializer);
            result.ContinueWith((ioResult) =>
            {
                Console.WriteLine($"[sender] Stream Completed, successful = {ioResult.Result.WasSuccessful}");
            });
        }
    }
}
