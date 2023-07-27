//-----------------------------------------------------------------------
// <copyright file="ICompressionAlgorithm.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is the interface for all compression types used in the archiver. A class that implements this interface must contain ways to compress and decomppress files with their respectiv algorithms. It also contains a name for the
// compression. Addionally a method to calculate the size of the resulting file, after its compression is included.
// </summary>
//-----------------------------------------------------------------------
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