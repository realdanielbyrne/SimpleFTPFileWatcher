using System;
using System.IO;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace issFTP.Services {

  public interface IFtpService {
    void SendFile (string name, string fullPath);
  }

  public class FtpService : IFtpService {
    private readonly AppSettings _settings;
    private readonly ILogger<FtpService> _logger;
    public FtpService (ILogger<FtpService> logger, IOptions<AppSettings> settings) {
      _settings = settings.Value;
      _logger = logger;
    }
    public void SendFile (string name, string fullPath) {
      if (File.Exists (fullPath)) {
        
        var remotepath = string.IsNullOrEmpty(_settings.FtpRemote.RemoteDirectory) 
                          ? $"ftp://{_settings.FtpRemote.Address}/{name}"
                          : $"ftp://{_settings.FtpRemote.Address}/{_settings.FtpRemote.RemoteDirectory}/{name}";
        try {
            _logger.LogInformation ("Starting ftp upload.\n");

            // Get the object used to communicate with the server
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remotepath);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_settings.FtpRemote.Username, _settings.FtpRemote.Password);

            // Copy the contents of the file to the request stream
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(fullPath))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }
            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
              _logger.LogInformation ($"Upload File Complete, status {response.StatusDescription}");
            }

        } catch (Exception ex) {
          _logger.LogError (ex, ex.Message);
        }
      } else
        _logger.LogWarning ($"File Not Found : {name}.  Ensure the path, {fullPath}, is correct.");
    }
  }
}