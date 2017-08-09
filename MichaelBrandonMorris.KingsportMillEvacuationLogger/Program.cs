using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger
{
    /// <summary>
    ///     Class Program.
    /// </summary>
    /// TODO Edit XML Comment Template for Program
    public class Program
    {
        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// TODO Edit XML Comment Template for Main
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder().UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSetting("detailedErrors", "true")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .CaptureStartupErrors(true)
                .Build();

            host.Run();
        }
    }
}