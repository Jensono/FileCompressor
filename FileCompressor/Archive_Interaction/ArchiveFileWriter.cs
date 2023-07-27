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



        public ArchiveFileWriter(string destination, string archiveName, ICompressionAlgorithm compressionAlgorithm)
        {
            this.DestinationFolder = destination;
            this.ArchiveName = archiveName;
            this.CompressionAlgorithm = compressionAlgorithm;
        }

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
        
        public void CreateArchive(ArchiveHeader archiveHeader, List<FileMetaInformation> filesToBeWrittenIntoArchive)
        {
            // TODO // TODO VALIDATE THAT GIVEN ARCHIVENAME IS IN FACT NOT A PATH BUT A FILENAME that is to be created, it just worked with a path, saying that it was the same path as the destination folder.
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
            catch (UnauthorizedAccessException e)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Could not read file with filepath: {archiveFilePath} .");
            }
            catch (Exception e)
            {            // TODO Specify more
                throw e;
            }
        }

        private void AppendFileWithFileHeaderToArchive(string archiveFilePath, FileMetaInformation fileInfo, long compressedSizeOfFile)
        {
            this.WriteFileHeaderToArchive(archiveFilePath, fileInfo, compressedSizeOfFile);

            this.CompressionAlgorithm.Compress(fileInfo.FullName, archiveFilePath);

            //////////////////////////////////REVISE THE FILEHEADER TO CONTAIN ACCURATE INFORMATION ON THE FILE
        }

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
            catch (UnauthorizedAccessException e)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Could not read file with filepath: {archiveFilePath}. File may be read only.");
            }
            catch (Exception e)
            {            // todo Specify more

                throw e;
            }
        }

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
            catch (UnauthorizedAccessException e)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Could not read file with filepath: {archiveFilePath}. File may be read only.");
            }
            catch (DirectoryNotFoundException e)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Could not read part of the Filepath: {archiveFilePath}. Does given directory already exist?.");
            }
            catch (Exception e)
            {            // todo Specify more

                throw e;
            }
        }

        // returns true if the destination folder contains enough space for the compression.
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

            string driveLetter = Path.GetPathRoot(this.DestinationFolder).Substring(0, 1);
            DriveInfo driveInfo = new DriveInfo(driveLetter);

            if (driveInfo.AvailableFreeSpace > sumExpectedFileSize)
            {
                return true;
            }

            return false;
        }

    }
}