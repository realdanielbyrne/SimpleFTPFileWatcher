# issFTP

Industry Safe SFTP Client and File watcher.  This client will watch a directory for new files, upload these new files to the IndustrySafe SFTP server, and then Delete the uploaded files.

issFTP is designed to be run as a Windows service, although it can be run as a 
console application as well.


## Building

This is a dotnet core 3.1 project. [Download](https://dotnet.microsoft.com/download/dotnet-core) and install the dotnet core SDK to build this application.
[Visual Studio Code](https://code.visualstudio.com/) is reccomended for editing and debugging, but any editor will work. This application was only tested on Windows, but operation on other runtimes should be possible.  

issFTP requires the SSH.NET library.  You can restore the required dependencies from the .NET cli.

    dotnet restore

To build the application.

    dotnet build


## Publishing

To build a self contained version of the application for Windows x64.

    dotnet publish --runtime win-x64

To publish this application as a Windows service, copy the publish directory to a directory of your choice, and then install the service using the sc command from an elevated Windows command prompt.

Create the service with

    sc create issFTP binpath="{full path to publish folder}/issftp.exe"

Start the service with

    sc start issFTP

Stop the service with 

    sc stop issFTP

Delete the service with 

    sc delete issFTP


## Configuration

Configuration is handled in the appsettings.json file.  This section of note is the AppSettings section.  Here you can change the local folder to be watched, the remote host, the credentials to the remote host,
the allowed filenames, and the directory on the remote where the files will be uploaded.

    "AppSettings": {
      "LocalFolder": "D:\\sftp",
      "Username" : "rpmasset",
      "Password" : "QKa3Rbz7!i9KXlDy8",
      "SFTPUri": "sftp.industrysafe.com",
      "EmployeesFile" : "RPMEMP.TXT",
      "FacilitiesFile" : "RPMFAC.TXT",
      "AssetsFile" : "RPMASSETS.TXT",
      "RemoteDirectory":"incoming"
    }

## Hosting

The service is currently hosted on the server `rpmsql02.rpmx.local` in the `c:\issftp\publish` folder.

