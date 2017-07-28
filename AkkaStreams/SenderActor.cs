using System;
using System.IO;
using System.Threading.Tasks;
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
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(3), Self, new Messages.StartSending(), Self);

            ReceiveAsync<Messages.StartSending>(ReceivedStartSending);
        }

        private async Task ReceivedStartSending(Messages.StartSending message)
        {
            Console.WriteLine($"[sender] ReceivedStartSending()");
            var stream = new FileStream(FILENAME, FileMode.Open);

            var materializer = Context.Materializer();
            var source = StreamConverters.FromInputStream(() => stream, CHUNK_SIZE);
            var result = source.To(Sink.ActorRef<ByteString>(this.receiver, new Messages.StreamComplete()))
                .Run(materializer);
            await result.ContinueWith((ioResult) =>
            {
                Console.WriteLine($"[sender] Stream Completed, successful = {ioResult.Result.WasSuccessful}");
            });

            var sel = Context.ActorSelection("akka.tcp://another-local-system@127.0.0.1:8092/user/receiver-one");
            sel.Tell(new Messages.Hail());
            var remoteRef = await sel.ResolveOne(TimeSpan.FromSeconds(5));

            var remoteResult = source.To(Sink.ActorRef<ByteString>(remoteRef, new Messages.StreamComplete()))
                .Run(materializer);
            await remoteResult.ContinueWith((ioResult) =>
            {
                Console.WriteLine($"[sender] Stream to Remote completed, successful = {ioResult.Result.WasSuccessful}");
            });
        }
    }
}
