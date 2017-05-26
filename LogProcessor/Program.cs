using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace LogProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                //.UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
