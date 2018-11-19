using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace aspnetcorebackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(opt => opt.ValidateScopes = false); // Needed when InMemory is in used
    }
}