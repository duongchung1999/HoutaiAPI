using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace template_program_backstage_web_api {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.Inject().UseStartup<Startup>();
                });
    }
}
