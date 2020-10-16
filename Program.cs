using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.EventLog;
using SimpleFtpFileWatcher.Services;

namespace SimpleFtpFileWatcher {
  public class Program {
    public static void Main (string[] args) {
      CreateHostBuilder (args)
        .Build()
        .Run();
    }

    public static IHostBuilder CreateHostBuilder (string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureServices ((hostContext, services) => 
        {
          services.Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings"));  
          services.AddScoped<ISftpService, SftpService>();        
          services.AddScoped<IFtpService, FtpService>();        
          
          services.AddHostedService<Worker>()
              .Configure<EventLogSettings> (config => {
                config.LogName = "Simple FTP FileWatcher";
                config.SourceName = "Simple FTP FileWatcher Source";      
              });
        })
        .UseWindowsService();
  }
}