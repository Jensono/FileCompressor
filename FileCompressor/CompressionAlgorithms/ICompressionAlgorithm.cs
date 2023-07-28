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

    /// <summary>
    /// This class is the interface for all compression types used in the archive application. A class that implements this interface must contain ways to compress and decompress files with their respective algorithms. It also contains a name for the
    /// compression. Additionally a method to calculate the size of the resulting file, after its compression is included.
    /// </summary>
    public interface ICompressionAlgorithm
    {
        /// <summary>
        /// This method compresses a files that is found given by the path and outputs it into a archive file that is located at the output path.
        /// </summary>
        /// <param name="inputOriginalFilePath"> The path to the original file that should be compressed.</param>
        /// <param name="outputArchiveFilePath"> The path to the archive file into which the file should be compressed.</param>
        void Compress(string inputOriginalFilePath, string outputArchiveFilePath);

        /// <summary>
        /// This method decompresses a file that is inside a archive file and outputs it into a directory that was specified.
        /// </summary>
        /// <param name="archiveFileStream"> The file stream from the archive file from which to decompress a file.</param>
        /// <param name="outputNewFilePath"> The output path to the new file that should be extracted.</param>
        /// <param name="decompressionStartIndexInFile"> The index byte inside the archive file or file stream from where to extract the file.</param>
        /// <param name="fileHeader"> The file header of the file that should be decompressed.</param>
        void Decompress(FileStream archiveFileStream, string outputNewFilePath, long decompressionStartIndexInFile, IndividualFileHeaderInformation fileHeader);

        /// <summary>
        /// This method returns the compression calling of the compression algorithm used. For example the current compression calling for no algorithm is "None".
        /// </summary>
        /// <returns> The compression calling as a string.</returns>
        string ReturnCompressionTypeCalling();

        /// <summary>
        /// This method returns the expected data size for a file (for which the file path is given), when it would be compressed with this algorithm.
        /// </summary>
        /// <param name="inputOriginalFilePath"> The file path to the orignal file.</param>
        /// <returns> The expected file size of the file after it will be compressed in bytes.</returns>
        long ReturnExpectedDataSizeCompressed(string inputOriginalFilePath);
    }
}