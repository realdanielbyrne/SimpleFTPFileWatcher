# SimpleFtpFileWatcher

Simple (s)FTP Client and File watcher.  This client will watch a directory for new files, upload these new files to the and SFTP and/or a FTP server, and then Delete or timestamp the uploaded files.

SimpleFtpFileWatcher is designed to be run as a Windows service, although it can be run as a 
console application as well.


## Building

This is a dotnet core 3.1 project. [Download](https://dotnet.microsoft.com/download/dotnet-core) and install the dotnet core SDK to build this application.
[Visual Studio Code](https://code.visualstudio.com/) is reccomended for editing and debugging, but any editor will work. This application was only tested on Windows, but operation on other runtimes should be possible.  

SimpleFtpFileWatcher requires the SSH.NET library.  You can restore the required dependencies from the .NET cli.

    dotnet restore

To build the application.

    dotnet build


## Publishing

To build a self contained version of the application for Windows x64.

    dotnet publish --runtime win-x64

To publish this application as a Windows service, copy the publish directory to a directory of your choice, and then install the service using the sc command from an elevated Windows command prompt.

Create the service with

    sc create SimpleFtpFileWatcher binpath="{full path to publish folder}/SimpleFtpFileWatcher.exe"

Start the service with

    sc start SimpleFtpFileWatcher

Stop the service with 

    sc stop SimpleFtpFileWatcher

Delete the service with 

    sc delete SimpleFtpFileWatcher


## Configuration

Configuration is handled in the appsettings.json file.  This section of note is the AppSettings section.  
Here you can change the local folder to be watched, the remote host, the credentials to the remote host,
the filenames or file extensions to watch for, and the directory on the remote where the files will be uploaded.

      "AppSettings": {
        "DeleteAfterUpload":"False",
        "WatchedFolder":"c:\\watched",
        "FtpRemote": 
        {
          "Address":"127.0.0.1",
          "Username":"tester",
          "Password":"password",
          "Type":"ftp",
          "FilesToWatch":["*.txt"],
          "RemoteDirectory":""
        },
        "SftpRemote":
        {
          "Address":"127.0.0.1",
          "Username":"tester",
          "Password":"password",
          "Type":"sftp",
          "FilesToWatch":["*.csv"],
          "RemoteDirectory":""
        }
      }


## Debugging Tools

Tested against the two following development servers:

1. [Filezilla Ftp Server](https://filezilla-project.org/download.php?type=server)
2. [Rebex Tiny Sftp Server](https://labs.rebex.net/tiny-sftp-server)


## License

The MIT License (MIT)
Copyright © 2020 Daniel Byrne

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

