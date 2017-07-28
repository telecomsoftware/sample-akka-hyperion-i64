using System;
using System.Diagnostics;
using Topshelf;

namespace RemoteReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Topshelf service");

            HostFactory.Run(cfg =>
            {
                cfg.Service<Service>(s =>
                {
                    s.ConstructUsing(name => new Service());
                    s.WhenStarted((service, host) => { return service.Start(host); });
                    s.WhenStopped((service) => service.Stop());
                });
                cfg.SetDisplayName("AkkaStreams");
                cfg.SetServiceName("AkkaStreams");
            });

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press [Enter] to exit");
                Console.ReadLine();
            }
        }
    }
}
