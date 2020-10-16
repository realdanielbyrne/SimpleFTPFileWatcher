using System;
using System.IO;

namespace SimpleFtpFileWatcher {
  public static class FileExtensions {
    public static string AppendTimeStamp (this string fileName) {

      var path = Path.GetDirectoryName(fileName);
      var fi = new FileInfo(fileName);
      var ext = fi.Extension.ToLower();
      
      fileName = Path.Combine(path,Path.GetFileNameWithoutExtension(fileName));
      

      // delete files over 7 days old
      string[] files = Directory.GetFiles(path);
      foreach (string file in files)
      {
        fi = new FileInfo(file);
        if (fi.LastAccessTime < DateTime.Now.AddDays(-7) && 
              (fi.Extension.ToLower() != ext))
            fi.Delete();
      }

      return string.Concat (        
        fileName,
        ".",
        DateTime.Now.ToString ("yyyyMMdd")
      );
    }
  }
}