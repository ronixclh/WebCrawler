using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebCrawlerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.ListenAnyIP(80);  
                        options.ListenAnyIP(5209);  
                        options.ListenAnyIP(7089, listenOptions =>
                        {
                            listenOptions.UseHttps(); 
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                   
                });
    }
}
