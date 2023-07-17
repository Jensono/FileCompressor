namespace FileCompressor
{
    internal class ExtractArchiveCommand
    {
        public ExtractArchiveCommand(string archiveSource, string destination,ICompressionAlgorithm compressionAlgorithm)

        {
            ArchiveFileReader archiveReader = new ArchiveFileReader(archiveSource,compressionAlgorithm);
            archiveReader.ExtractArchiveFiles(destination);
        }
    }
}