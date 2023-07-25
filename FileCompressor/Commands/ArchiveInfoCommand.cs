

namespace FileCompressor
{
    using System.IO;
    public class ArchiveInfoCommand : IArchiveCommand
    {
        // TODO FIELDS AND SHIT
        public string ArchiveSource { get; set; }

        public ArchiveInfoCommand(string source)
        {
            this.ArchiveSource = source;
        }

        public bool Execute()
        {
            if (File.Exists(this.ArchiveSource))
            {
                try
                {
                    this.ReadAndConvertArchiveHeader();
                }
                catch (ArchiveErrorCodeException e)
                {
                    throw e;
                }
            }
            else
            { // TODO CONVERT TO ERROR CODE
                throw new ArchiveErrorCodeException($"The file at {this.ArchiveSource} was not found. ");
            }

            return true;
        }

        // for reading the file header we dont need a specific compressionAlogrithm as all headers are written the same, regardeless of compression
        private void ReadAndConvertArchiveHeader()
        {
            try
            {
                ArchiveFileReader archiveReader = new ArchiveFileReader(this.ArchiveSource);
                archiveReader.ReadArchiveHeaderAndPrintToConsole();
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }
        }
    }
}