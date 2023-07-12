using System.IO;

namespace FileCompressor
{
    public class FileMetaInformation
    {




        public FileMetaInformation(FileInfo fileInfo, string relativePathOfFileInsideArchive) 
        {
            this.FileInfo = fileInfo;
            this.RelativePathForArchive = relativePathOfFileInsideArchive;
        }

        public FileInfo FileInfo { get; private set; }
        public string RelativePathForArchive { get; private set; }
    }
}