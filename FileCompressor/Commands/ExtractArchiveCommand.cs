namespace FileCompressor
{
    internal class ExtractArchiveCommand
    {
        public ExtractArchiveCommand(string archiveSource, string destination)

        {
            ArchiveFileReader archiveReader = new ArchiveFileReader(archiveSource);
            archiveReader.ExtractArchiveFiles(destination);
        }
    }
}