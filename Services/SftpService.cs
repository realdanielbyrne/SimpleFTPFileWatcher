using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace SimpleFtpFileWatcher.Services {

  public interface ISftpService {
    void SendFile (string name, string fullPath);
  }

  public class SftpService : ISftpService {
    private readonly AppSettings _settings;
    private readonly ILogger<SftpService> _logger;

    public SftpService (ILogger<SftpService> logger, IOptions<AppSettings> settings) {
      _settings = settings.Value;
      _logger = logger;
    }
    public void SendFile (string name, string fullPath) {

      if (File.Exists (fullPath)) {
        
        using (var sftp = new SftpClient (_settings.SftpRemote.Address, _settings.SftpRemote.Username, _settings.SftpRemote.Password)) {
          _logger.LogInformation ("Connecting to Sftp Server");
          sftp.Connect ();

          _logger.LogInformation ("Creating FileStream object to stream a file");
          using (var uplfileStream = System.IO.File.OpenRead (fullPath)) {
            if (!string.IsNullOrEmpty(_settings.SftpRemote.RemoteDirectory))
              sftp.ChangeDirectory (_settings.SftpRemote.RemoteDirectory);
            
            if (sftp.Exists(name)) 
              sftp.DeleteFile(name);
            
            sftp.UploadFile (uplfileStream, name, true);
          }
          sftp.Dispose ();
        }

      } else
        _logger.LogWarning ($"File Not Found : {name}.  Ensure the path, {fullPath}, is correct.");
    }
  }
}