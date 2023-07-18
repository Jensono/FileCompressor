using System.IO;

namespace FileCompressor
{
    public interface ICompressionAlgorithm
    {
        void Compress(string inputOriginalFilePath, string outputArchiveFilePath);

        void Decompress(FileStream archiveFileStream, string outputNewFilePath,long decompressionStartIndexInFile, IndividualFileHeaderInformation fileHeader);

        string CompressionTypeCalling();

        long ReturnExpectedDataSizeCompressed(string inputOriginalFilePath);
    }
}