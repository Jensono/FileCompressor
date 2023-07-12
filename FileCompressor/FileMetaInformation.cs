using System.IO;

namespace FileCompressor
{
    public class FileMetaInformation
    {




        public FileMetaInformation(FileInfo fileInfo, string relativePathOfFileInsideArchive) 
        {
            this.FullName = fileInfo.FullName;
            this.Name = fileInfo.Name;
            this.Length = fileInfo.Length;
            this.RelativePathForArchive = relativePathOfFileInsideArchive;
        }
        // constructor used when reading the fileinformation from a archive file OBSOLE I USED THE INDIVUDLA FILE HEADER FOR THAT INFORMATION
        //public FileMetaInformation(string relativePathOfFile, string fileName, string fullFileName, long lengthOfFile ) 
        //{

        //    this.FullName = fullFileName;
        //    this.Name = fileName;
        //    this.Length = lengthOfFile;
        //    this.RelativePathForArchive = relativePathOfFile;
        //}

        
        public string FullName { get; private set; }
        public string Name { get; private set; }
        public long Length { get; private set; }
        public string RelativePathForArchive { get; private set;}
    }
}