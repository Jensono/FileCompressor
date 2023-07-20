namespace FileCompressor
{
    internal class ExtractArchiveCommand : IArchiveCommand
    {
        public string DestinationPathToDirectory { get; set; }
        public string ArchiveSource { get; set; }

        public ExtractArchiveCommand(string archiveSource, string destination)

        {
            this.DestinationPathToDirectory = destination;
            this.ArchiveSource = archiveSource;

        }

        public bool Execute()
        {
            try
            {

                ArchiveFileReader archiveReader = new ArchiveFileReader(this.ArchiveSource);

                archiveReader.ExtractArchiveFiles(this.DestinationPathToDirectory);
            }
            catch (ArchiveErrorCodeException e)
            {
                return false;
                throw e;
            }
            return true;
            
        }
    }
}