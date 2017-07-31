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

            var sel = Context.ActorSelection("akka.tcp://another-local-system@127.0.0.1:8092/user/receiver-one");
            var remoteRef = await sel.ResolveOne(TimeSpan.FromSeconds(5));
            remoteRef.Tell(new Messages.Hail());

            // these two lines simulate the problem without using Akka.Streams at all
            //remoteRef.Tell(new ObjectDisposedException("stream", "already disposed"));
            //return;

            var stream = new FileStream(FILENAME, FileMode.Open);
            //stream.Close(); // causes Akka.Actor.Status.Failure with ObjectDisposedException which causes error

            var materializer = Context.Materializer();

            var source = StreamConverters.FromInputStream(() => stream, CHUNK_SIZE)
                .Select(bs => bs.ToArray());

            // single output local
            /*var localResult = source
                .To(Sink.ActorRef<byte[]>(this.receiver, new Messages.StreamComplete()))
                .Run(materializer);*/

            // single output remote
            /*var remoteResult = source
                .To(Sink.ActorRef<byte[]>(remoteRef, new Messages.StreamComplete()))
                .Run(materializer);*/

            // graph-based multiplex to local and remote
            var localSink = Sink.ActorRef<byte[]>(this.receiver, new Messages.StreamComplete());
            var remoteSink = Sink.ActorRef<byte[]>(remoteRef, new Messages.StreamComplete());
            var sink = Sink.Combine(i => new Broadcast<byte[]>(i), localSink, remoteSink);
            //source.RunWith(sink, materializer);
        }
    }
}
