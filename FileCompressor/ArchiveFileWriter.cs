using System;
using System.Collections.Generic;
using System.IO;

namespace FileCompressor
{
    internal class ArchiveFileWriter
    {
        //source and destination. Then it writes has a method for c
        //TODO WRITE PROPERTIES AND FIELDS
        private string DestinationFolder;

        private string ArchiveName;
        private ICompressionAlgorithm CompressionAlgorithm;

        public ArchiveFileWriter(string destination, string archiveName, ICompressionAlgorithm compressionAlgorithm)
        {
            this.DestinationFolder = destination;
            this.ArchiveName = archiveName;
            this.CompressionAlgorithm = compressionAlgorithm;
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

        private void ChangeArchiveHeaderToNewHeader(string archiveFilePath, ArchiveHeader newModifiedArchiveHeader)
        {
            byte[] newArchiveHeaderAsBytes = newModifiedArchiveHeader.GetArchiveHeaderAsBytes();

            try
            {
                //Just overwrite the old ArchiveHeader
                using (FileStream fs = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Write))
                {
                    fs.Write(newArchiveHeaderAsBytes, 0, newArchiveHeaderAsBytes.Length);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Could not read file with filepath: {archiveFilePath} .");
            }
            //Specify more
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateArchive(ArchiveHeader archiveHeader, List<FileMetaInformation> filesToBeWrittenIntoArchive)
        {
            //TODO // TODO VALIDATE THAT GIVEN ARCHIVENAME IS IN FACT NOT A PATH BUT A FILENAME that is to be created, it just worked with a path, saying that it was the same path as the destination folder.
            string archiveFilePath = Path.Combine(DestinationFolder, ArchiveName);

            long[] expectedFileSizes = this.ReturnCompressedSizeForFilesAsArray(filesToBeWrittenIntoArchive);
            //CHECKING FOR DISK SPACE ON THE DISK that houses the desired archive directory
            if (!this.CheckExpectedFileSizeForAppend(filesToBeWrittenIntoArchive, expectedFileSizes))
            {
                throw new ArchiveErrorCodeException("Errorcode 1. Not enough diskspace to append to this archive!");
            }

            // creating a new file or overwritting the old one with this filemode

            

            //write the archive header to the file
            this.WriteArchiveHeaderToFile(archiveFilePath, archiveHeader);

            for (int i = 0; i < filesToBeWrittenIntoArchive.Count; i++)
            {
                FileMetaInformation fileInfo = filesToBeWrittenIntoArchive[i];
                this.AppendFileWithFileHeaderToArchive(archiveFilePath, fileInfo, expectedFileSizes[i]);
                //is it  even necessary to close the streams when im ALREADY use using?
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
            //Specify more
            catch (Exception e)
            {
                throw e;
            }
        }

        private void WriteArchiveHeaderToFile(string archiveFilePath, ArchiveHeader archiveHeader)
        {
            try
            {
                //TODO CHECK IF FILE ALREADY EXISTS MAYBE?
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
            //Specify more
            catch (Exception e)
            {
                throw e;
            }
        }

        //returns true if the destination folder contains enough space for the compression.
        private bool CheckExpectedFileSizeForAppend(List<FileMetaInformation> fileMetaInformationList, long[] expectedSizesForFiles)
        {
            long sumExpectedFileSize = 0;

            //add the archiveheadersize

            long counter = 0;
            sumExpectedFileSize += new FixedVariables().ArchiveHeaderLength;
            foreach (var item in fileMetaInformationList)
            {
                IndividualFileHeaderInformation header;
                //get the length of each individual file header
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

        public long[] ReturnCompressedSizeForFilesAsArray(List<FileMetaInformation> fileMetaInformationList)
        {
            long[] expectedSize = new long[fileMetaInformationList.Count];

            //add the archiveheadersize

            for (int i = 0; i < fileMetaInformationList.Count; i++)
            {
                expectedSize[i] = this.CompressionAlgorithm.ReturnExpectedDataSizeCompressed(fileMetaInformationList[i].FullName);
            }

            return expectedSize;
        }
    }
}