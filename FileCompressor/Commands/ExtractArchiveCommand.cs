using System;

namespace FileCompressor
{
    public class ExtractArchiveCommand : IArchiveCommand
    {
        private string destinationPathToDirectory;
        private string archiveSource;

        public string DestinationPathToDirectory
        {
            get
            {
                return this.destinationPathToDirectory;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.DestinationPathToDirectory)} cannot be null!");
                }

                this.destinationPathToDirectory = value;
            }
        }

        public string ArchiveSource
        {
            get
            {
                return this.archiveSource;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ArchiveSource)} cannot be null!");
                }

                this.archiveSource = value;
            }
        }

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
                throw e;

                ////return false;
            }

            return true;
        }
    }
}