//-----------------------------------------------------------------------
// <copyright file="ArchiveFileWriter.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is used to create and append archive files. It contains operations to append all the crucial parts that make up the meta information in the archive itself.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// This class is used to create and append archive files. It contains operations to append all the crucial parts that make up the meta information in the archive itself.
    /// </summary>
    public class ArchiveFileWriter
    {
        /// <summary>
        /// The field for the path of the destination folder from which to compress files from.
        /// </summary>
        private string destinationFolder;

        /// <summary>
        /// The field for the compression algorithm used to compress files.
        /// </summary>
        private ICompressionAlgorithm compressionAlgorithm;

        /// <summary>
        /// The field for the name of the archive.
        /// </summary>
        private string archiveName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveFileWriter"/> class. 
        /// </summary>
        /// <param name="destination"> The destination path to where the archive should be written to. </param>
        /// <param name="archiveName"> The name the archive should have as a string.</param>
        /// <param name="compressionAlgorithm"> The compression that should be used within the archive.</param>
        public ArchiveFileWriter(string destination, string archiveName, ICompressionAlgorithm compressionAlgorithm)
        {
            this.DestinationFolder = destination;
            this.ArchiveName = archiveName;
            this.CompressionAlgorithm = compressionAlgorithm;
        }

        /// <summary>
        /// Gets or sets the path of the destination folder from which to compress files from.
        /// </summary>
        /// <value> The path of the destination folder from which to compress files from. </value>
        public string DestinationFolder
        {
            get
            {
                return this.destinationFolder;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.DestinationFolder)} cannot be null!");
                }

                this.destinationFolder = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the archive.
        /// </summary>
        /// <value> The name of the archive. </value>
        public string ArchiveName
        {
            get
            {
                return this.archiveName;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ArchiveName)} cannot be null!");
                }

                this.archiveName = value;
            }
        }

        /// <summary>
        /// Gets or sets the compression algorithm used to compress files.
        /// </summary>
        /// <value> The compression algorithm used to compress files. </value>
        public ICompressionAlgorithm CompressionAlgorithm
        {
            get
            {
                return this.compressionAlgorithm;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.CompressionAlgorithm)} cannot be null!");
                }

                this.compressionAlgorithm = value;
            }
        }

        /// <summary>
        /// This method appends a list of files to an already existing archive.
        /// </summary>
        /// <param name="archiveFilePath"> The path to the Archive file.</param>
        /// <param name="filesToBeWrittenIntoArchive"> The list of files that should be appended to the archive.</param>
        /// <param name="newModifiedArchiveHeader"> The new archive header that needs to overwrite the old archive header. Filled with adjusted information for the append.</param>
        /// <exception cref="ArchiveErrorCodeException"> Is raised when there is not enough disk space on the drive that contains the archive file.</exception>
        public void AppendToArchive(string archiveFilePath, List<FileMetaInformation> filesToBeWrittenIntoArchive, ArchiveHeader newModifiedArchiveHeader)
        {
            long[] expectedFileSizes = this.ReturnCompressedSizeForFilesAsArray(filesToBeWrittenIntoArchive);

            if (!this.CheckExpectedFileSizeForAppend(filesToBeWrittenIntoArchive, expectedFileSizes))
            {
                throw new ArchiveErrorCodeException("Errorcode 1. Not enough diskspace to append to this archive!");
            }

            // one could even make a method that just appends a lot of files
            for (int i = 0; i < filesToBeWrittenIntoArchive.Count; i++)
            {
                FileMetaInformation fileInfo = filesToBeWrittenIntoArchive[i];
                this.AppendFileWithFileHeaderToArchive(archiveFilePath, fileInfo, expectedFileSizes[i]);
            }

            this.ChangeArchiveHeaderToNewHeader(archiveFilePath, newModifiedArchiveHeader);
        }

        /// <summary>
        /// This method create a new archive file. The archive will also be in the same folder that was specified for compression.
        /// </summary>
        /// <param name="archiveHeader"> The archive header for the archive file.</param>
        /// <param name="filesToBeWrittenIntoArchive"> The files that should be contained within the archive file.</param>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown when there is not enough disk space on the specified drive for the archive.</exception>
        public void CreateArchive(ArchiveHeader archiveHeader, List<FileMetaInformation> filesToBeWrittenIntoArchive)
        {
           
            string archiveFilePath = Path.Combine(this.DestinationFolder, this.ArchiveName);

            long[] expectedFileSizes = this.ReturnCompressedSizeForFilesAsArray(filesToBeWrittenIntoArchive);

            // checking available disk space on the drive that holds the desired archive directory
            if (!this.CheckExpectedFileSizeForAppend(filesToBeWrittenIntoArchive, expectedFileSizes))
            {
                throw new ArchiveErrorCodeException("Errorcode 1. Not enough diskspace to append to this archive!");
            }

            // creating a new file or overwritting the old one with this filemode

            // write the archive header to the file
            this.WriteArchiveHeaderToFile(archiveFilePath, archiveHeader);

            for (int i = 0; i < filesToBeWrittenIntoArchive.Count; i++)
            {
                FileMetaInformation fileInfo = filesToBeWrittenIntoArchive[i];
                this.AppendFileWithFileHeaderToArchive(archiveFilePath, fileInfo, expectedFileSizes[i]);
            }
        }

        /// <summary>
        /// This method returns a list of compressed file sizes the files will have in the archive.
        /// </summary>
        /// <param name="fileMetaInformationList"> The files that should be checked for their compressed size.</param>
        /// <returns> A array of long values for the assumed compressed size of the files.</returns>
        public long[] ReturnCompressedSizeForFilesAsArray(List<FileMetaInformation> fileMetaInformationList)
        {
            long[] expectedSize = new long[fileMetaInformationList.Count];

            // add the archiveheadersize
            for (int i = 0; i < fileMetaInformationList.Count; i++)
            {
                expectedSize[i] = this.CompressionAlgorithm.ReturnExpectedDataSizeCompressed(fileMetaInformationList[i].FullName);
            }

            return expectedSize;
        }

        /// <summary>
        /// This method modifies an archive files archive header to a new archive header.
        /// </summary>
        /// <param name="archiveFilePath"> The path to the archive file.</param>
        /// <param name="newModifiedArchiveHeader"> The new adjusted archive header that should overwrite the old archive header.</param>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown when the file, with the given file path, could not be read. </exception>
        private void ChangeArchiveHeaderToNewHeader(string archiveFilePath, ArchiveHeader newModifiedArchiveHeader)
        {
            byte[] newArchiveHeaderAsBytes = newModifiedArchiveHeader.GetArchiveHeaderAsBytes();

            try
            {
                // Just overwrite the old ArchiveHeader
                using (FileStream fs = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Write))
                {
                    fs.Write(newArchiveHeaderAsBytes, 0, newArchiveHeaderAsBytes.Length);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Could not read file with filepath: {archiveFilePath} .");
            }
        }

        /// <summary>
        /// This method appends a <see cref="IndividualFileHeaderInformation"/> and the file itself to an existing archive.
        /// </summary>
        /// <param name="archiveFilePath"> The path to the archive the new data should be appended to.</param>
        /// <param name="fileInfo"> The file info for the file that should be appended.</param>
        /// <param name="compressedSizeOfFile"> The presumed compressed size of the file that should be appended.</param>
        private void AppendFileWithFileHeaderToArchive(string archiveFilePath, FileMetaInformation fileInfo, long compressedSizeOfFile)
        {
            this.WriteFileHeaderToArchive(archiveFilePath, fileInfo, compressedSizeOfFile);

            this.CompressionAlgorithm.Compress(fileInfo.FullName, archiveFilePath);

            //////////////////////////////////REVISE THE FILEHEADER TO CONTAIN ACCURATE INFORMATION ON THE FILE
        }

        /// <summary>
        /// This Method writes the individual file header as bytes into the archive file.
        /// </summary>
        /// <param name="archiveFilePath"> The path to the archive file that should be written.</param>
        /// <param name="fileInfo"> The file info of the file that the file header should be written for. </param>
        /// <param name="compressedFileSize"> The assumed compressed size of the file in the archive.</param>
        /// <exception cref="ArchiveErrorCodeException"> 
        /// Is thrown when: the file, with the given file path, could not be read.
        ///                 If an archive with the same name already exists, is read only and can not be overwritten.                 
        /// </exception>
        private void WriteFileHeaderToArchive(string archiveFilePath, FileMetaInformation fileInfo, long compressedFileSize)
        {
            byte[] fileHeaderBytes;

            // get the file header as a byte array and write it into the file
            try
            {
                fileHeaderBytes = new IndividualFileHeaderInformation(fileInfo.Name, fileInfo.RelativePathForArchive, fileInfo.Length, compressedFileSize).GetFileHeaderAsByteArray();
            }
            catch (ArgumentNullException)
            {
                throw new ArchiveErrorCodeException("Errorcode 1. Given Source may be corrupted! ");
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArchiveErrorCodeException("Errorcode 1. Given Source may be corrupted! ");
            }

            try
            {
                using (var archiveFileStream = new FileStream(archiveFilePath, FileMode.Append))
                {
                    archiveFileStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Could not read file with filepath: {archiveFilePath}. File may be read only.");
            }
        }

        /// <summary>
        /// This method write the archive header into the archive file.
        /// </summary>
        /// <param name="archiveFilePath"> The file path to which to write the archive header to.</param>
        /// <param name="archiveHeader"> The archive header that need to be written as bytes to the file.</param>
        private void WriteArchiveHeaderToFile(string archiveFilePath, ArchiveHeader archiveHeader)
        {
            try
            {
                using (var archiveFileStream = new FileStream(archiveFilePath, FileMode.Create))
                {
                    byte[] archiveHeaderBytes = archiveHeader.GetArchiveHeaderAsBytes();
                    archiveFileStream.Write(archiveHeaderBytes, 0, archiveHeaderBytes.Length);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Could not read file with filepath: {archiveFilePath}. File may be read only.");
            }
            catch (DirectoryNotFoundException)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Could not read part of the Filepath: {archiveFilePath}. Does given directory already exist?.");
            }
            catch (IOException)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Could not read part of the Filepath: {archiveFilePath}. A File might already be in use.");
            }
        }

        // returns true if the destination folder contains enough space for the compression.

        /// <summary>
        /// This method checks whether or not the drive into which to append to has enough space, so that the append can be performed.
        /// </summary>
        /// <param name="fileMetaInformationList"> The list of file meta information for all the files that need to be appended.</param>
        /// <param name="expectedSizesForFiles"> The expected compressed size for all files that need to be appended as a long array.</param>
        /// <returns> A boolean value indicating whether or not there is enough disk space for the planned append execution. </returns>
        ///  <exception cref="ArchiveErrorCodeException">  Is thrown when the individual file header can not be generated for a file. Its assumed the file information is corrupt.
        /// </exception>
        private bool CheckExpectedFileSizeForAppend(List<FileMetaInformation> fileMetaInformationList, long[] expectedSizesForFiles)
        {
            long sumExpectedFileSize = 0;

            // add the archiveheadersize
            long counter = 0;
            sumExpectedFileSize += new FixedVariables().ArchiveHeaderLength;
            foreach (var item in fileMetaInformationList)
            {
                IndividualFileHeaderInformation header;

                // get the length of each individual file header
                try
                {
                    header = new IndividualFileHeaderInformation(item.Name, item.RelativePathForArchive, item.Length, item.Length);
                }
                catch (ArgumentNullException)
                {
                    throw new ArchiveErrorCodeException("Errorcode 1. Given Source may be corrupted! ");
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ArchiveErrorCodeException("Errorcode 1. Given Source may be corrupted! ");
                }

                byte[] headerArray = header.GetFileHeaderAsByteArray();
                sumExpectedFileSize += headerArray.Length;

                sumExpectedFileSize += expectedSizesForFiles[counter];
                counter++;
            }

            DriveInfo driveInfo;
            try
            {
                string driveLetter = Path.GetPathRoot(this.DestinationFolder).Substring(0, 1);
                driveInfo = new DriveInfo(driveLetter);
            }
            catch (Exception)
            {
                // drive is assumed to be non existend.
                return false;
            }
          
            if (driveInfo.AvailableFreeSpace > sumExpectedFileSize)
            {
                return true;
            }

            return false;
        }
    }
}