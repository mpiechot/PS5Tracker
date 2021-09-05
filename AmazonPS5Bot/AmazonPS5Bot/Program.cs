using System;
using System.IO;
using System.Net;
using Topshelf;

namespace PS5Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(config =>
            {
                config.Service<PS5TrackerService>(s =>
                {
                    s.ConstructUsing(psTracker => new PS5TrackerService());
                    s.WhenStarted(psTracker => psTracker.Start());
                    s.WhenStopped(psTracker => psTracker.Stop());
                });
                config.RunAsLocalSystem();
                config.SetServiceName("PS5Tracker");
                config.SetDisplayName("PS5 Tracker");
                config.SetDescription("This Service tracks stores to order a PS5");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
