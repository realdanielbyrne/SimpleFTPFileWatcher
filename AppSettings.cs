using System;
using System.Collections.Generic;

namespace issFTP
{
    public class AppSettings
    {
        public AppSettings()
        {
            FtpRemote = new RemoteConnection();
            SftpRemote = new RemoteConnection();
        }
        public string WatchedFolder { get; set; }
        public bool DeleteAfterUpload { get; set; }
        public RemoteConnection FtpRemote { get; set; }
        public RemoteConnection SftpRemote { get; set; }
    }

    public class RemoteConnection {      
      public string Address { get; set; }
      public string Username { get; set; }
      public string Password { get; set; }    
      public string Type { get; set; }  
      public string[] FilesToWatch { get; set; }
      public string RemoteDirectory { get; set; }
    
    }
}