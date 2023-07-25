﻿
namespace FileCompressor
{
    using System.IO;

    public interface ICompressionAlgorithm
    {
        void Compress(string inputOriginalFilePath, string outputArchiveFilePath);

        void Decompress(FileStream archiveFileStream, string outputNewFilePath, long decompressionStartIndexInFile, IndividualFileHeaderInformation fileHeader);

        string ReturnCompressionTypeCalling();

        long ReturnExpectedDataSizeCompressed(string inputOriginalFilePath);
    }
}