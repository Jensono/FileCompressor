

namespace FileCompressor
{
    using System.IO;

    public class FileMetaInformation
    {
        public FileMetaInformation(FileInfo fileInfo, string relativePathOfFileInsideArchive)
        {
            this.FullName = fileInfo.FullName;
            this.Name = fileInfo.Name;
            this.Length = fileInfo.Length;
            this.RelativePathForArchive = relativePathOfFileInsideArchive;
        }

        public string FullName { get; private set; }
        public string Name { get; private set; }
        public long Length { get; private set; }
        public string RelativePathForArchive { get; private set; }
    }
}