﻿//-----------------------------------------------------------------------
// <copyright file="NoCompressionAlgorithm.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is the contains information on how to compress and decompress a file that uses no specific compression algorithm.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.IO;

    /// <summary>
    /// This class is the contains information on how to compress and decompress a file that uses no specific compression algorithm.
    /// </summary>
    public class NoCompressionAlgorithm : ICompressionAlgorithm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoCompressionAlgorithm"/> class. 
        /// </summary>
        public NoCompressionAlgorithm()
        {
        }

        // In theory a compress is just a append, because right now i dont need to compress at any other place other then

        /// <summary>
        /// This method compresses a file, from a given source path into an archive file with a destination path, with no compression at all.It just copies the files bytes into the archive file.
        /// </summary>
        /// <param name="inputOriginalFilePath"> The source path to the file which should be compressed.</param>
        /// <param name="outputArchiveFilePath"> The destination path to the archive into which the file should be written.</param>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown if there was a <see cref="IOException"/> caught during compression.</exception>
        public void Compress(string inputOriginalFilePath, string outputArchiveFilePath)
        {
            try
            {
                using (var archiveFileStream = new FileStream(outputArchiveFilePath, FileMode.Append))
                {                   
                    using (var originalFileFileStream = new FileStream(inputOriginalFilePath, FileMode.Open, FileAccess.Read))
                    {
                        int standartBufferLength = 1048576; // 1MB buffer
                        var buffer = new byte[standartBufferLength];
                       
                        //small todo: analyse the inner workings of these functions...
                        int bytesRead;

                        while ((bytesRead = originalFileFileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            archiveFileStream.Write(buffer, 0, bytesRead);
                        }

                        // is it  even necessary to close the streams when im ALREADY use using?
                        originalFileFileStream.Close();
                    }
                }
            }
            catch (IOException)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1, could not access the Files Inside the source");
            }
            catch (Exception e)
            {               
                throw e;
            }
        }

        // todo small: create a class or enum or something that holds these values, so they dont lose meaning over time.

        /// <summary>
        /// This method returns the compression calling for no compression algorithm.
        /// </summary>
        /// <returns> A string that indicates the type of compression is no compression.</returns>
        public string ReturnCompressionTypeCalling()
        {
            return new FixedVariables().CompressionCallingTypeNoCompression;
        }

        /// <summary>
        /// This method decompresses or extracts a File from a given archive file into a given directory.
        /// </summary>
        /// <param name="archiveFilestream"> The archive file stream associated with a archive file.</param>
        /// <param name="outputNewFilePath"> The output file path for the file that should be extracted.</param>
        /// <param name="archiveDecompressionStartPoint"> The index in the file stream or archive file from which to start reading the file content.</param>
        /// <param name="fileHeader"> The individual file header of the file.</param>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown if something unexpected happens during decompression.</exception>
        public void Decompress(FileStream archiveFilestream, string outputNewFilePath, long archiveDecompressionStartPoint, IndividualFileHeaderInformation fileHeader)
        {
            // Writting the new file here.
            int standartBufferLength = 1024;
            byte[] buffer = new byte[standartBufferLength];

            // create a new file with the output path.
            using (FileStream extratedNewFileStream = new FileStream(outputNewFilePath, FileMode.Create, FileAccess.Write))
            {
                archiveFilestream.Seek(archiveDecompressionStartPoint, SeekOrigin.Begin);

                // we read as long as we have found out we need to from the fileheader
                long bytesLeft = fileHeader.SizeCompressed;

                // read the archive contents in kilobit chunks and only start reading with less when nearing the end. Eg less than the usual buffer is left.
                while (bytesLeft > standartBufferLength)
                {
                    archiveFilestream.Read(buffer, 0, buffer.Length);
                    extratedNewFileStream.Write(buffer, 0, buffer.Length);
                    bytesLeft -= buffer.Length;
                }

                // read the last remaining bits before the filecontent ends in the archive.
                byte[] lastBuffer = new byte[bytesLeft];
                archiveFilestream.Read(lastBuffer, 0, lastBuffer.Length);
                extratedNewFileStream.Write(lastBuffer, 0, lastBuffer.Length);
            }
        }

        /// <summary>
        /// This method returns the expected size of a file compressed with no compression. Meaning it just returns the uncompressed file size in this case.
        /// </summary>
        /// <param name="inputOriginalFilePath"> The path to the original file for which to calculate the amount of bytes compressed.</param>
        /// <returns> The number of bytes the the file will have if compressed with this algorithm.</returns>
        public long ReturnExpectedDataSizeCompressed(string inputOriginalFilePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(inputOriginalFilePath);
                return fileInfo.Length;
            }
            catch (Exception e)
            {                
                throw e;
            }
        }
    }
}