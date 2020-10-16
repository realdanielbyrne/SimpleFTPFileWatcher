using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SimpleFtpFileWatcher.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SimpleFtpFileWatcher {
  public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger; 
    private FileSystemWatcher _folderWatcher;
    public AppSettings _settings;

    private readonly IServiceProvider _services;

    public Worker (ILogger<Worker> logger, IOptions<AppSettings> settings, IServiceProvider services) {
      _logger = logger;
      _settings = settings.Value;
      _services = services;
    }

    protected override async Task ExecuteAsync (CancellationToken stoppingToken) {
      await Task.CompletedTask;
    }

    public override Task StartAsync (CancellationToken cancellationToken) {
      
      var _inputFolder = _settings.WatchedFolder;

      if (!Directory.Exists (_inputFolder)) {
        _logger.LogCritical ($"Please make sure the folder [{_inputFolder}] exists, then restart the service.");
        return Task.CompletedTask;
      }
      
      _logger.LogInformation ("Starting SimpleFTP File Watcher Service");             
      _folderWatcher = new FileSystemWatcher () {
        NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName,
        IncludeSubdirectories =  false,
        Path = _inputFolder,
        EnableRaisingEvents = true
      };        

      if (string.IsNullOrEmpty(_settings.FtpRemote.Address) && 
          string.IsNullOrEmpty(_settings.SftpRemote.Address))
      {
        _logger.LogCritical ("No remote address specified.");
        return Task.CompletedTask;
      }

      foreach (var file in _settings.FtpRemote.FilesToWatch)
        _folderWatcher.Filters.Add(file);    

      foreach (var file in _settings.SftpRemote.FilesToWatch)
        _folderWatcher.Filters.Add(file);    

      _folderWatcher.Created += Input_OnChanged;
      return base.StartAsync (cancellationToken);
    }

    protected void Input_OnChanged (object source, FileSystemEventArgs e) {
      bool fileSent = false;
      try {
        if (e.ChangeType == WatcherChangeTypes.Created) {
      
          _logger.LogInformation ($"Created Event by [{e.FullPath}]");
          var fi = new FileInfo(e.Name);
          var ext = $"*{fi.Extension.ToLower()}";

          if (_settings.FtpRemote.FilesToWatch.Contains(e.Name) || 
              _settings.FtpRemote.FilesToWatch.Contains(ext)) {
            using (var scope = _services.CreateScope ()) {
              var ftpService = scope.ServiceProvider.GetRequiredService<IFtpService> ();
              ftpService.SendFile (e.Name, e.FullPath);
            }
            _logger.LogInformation ($"Sent {_settings.FtpRemote.Address}/{e.Name}");
            fileSent = true;
          }
          
          if (_settings.SftpRemote.FilesToWatch.Contains(e.Name) || 
              _settings.SftpRemote.FilesToWatch.Contains(ext)) {
            using (var scope = _services.CreateScope ()) {
              var sftpService = scope.ServiceProvider.GetRequiredService<ISftpService>();
              sftpService.SendFile (e.Name, e.FullPath);
            }
            _logger.LogInformation ($"Sent {_settings.SftpRemote.Address}/{e.Name}");
            fileSent = true;
          }
          
          if (_settings.DeleteAfterUpload && fileSent) {
            _logger.LogInformation ("Deleting file after upload.  To change this setting, modify appsettings.json.");
            File.Delete (e.FullPath);            
          } 
          else {
            _logger.LogInformation ("Appending date to uploaded file.");
            File.Move(e.FullPath,e.FullPath.AppendTimeStamp(),true);
          }
        }
      }

      catch (Exception ex) {
        _logger.LogError(ex,ex.Message);
      }
    }

    public override async Task StopAsync (CancellationToken cancellationToken) {
      _logger.LogInformation ("Stopping SimpleFTP File Watcher Service");
      _folderWatcher.EnableRaisingEvents = false;
      await base.StopAsync (cancellationToken);
    }

    public override void Dispose () {
      _folderWatcher.Dispose ();
      base.Dispose ();
    }
  }
}