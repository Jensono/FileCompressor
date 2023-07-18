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

        //public void CreateArchive(ArchiveHeader archiveHeader, List<FileMetaInformation> filesToBeWrittenIntoArchive)
        //{
        //    string archiveFilePath = Path.Combine(DestinationFolder, ArchiveName);

        //    // creating a new file or overwritting the old one with this filemode

        //    ///TODO !!!!! TRY CATCH SO THAT WHEN SOMEBODY ELSE USES THE FILE IT DOESNT THROW SHIT LIKE AN APE
        //    using (var archiveFileStream = new FileStream(archiveFilePath, FileMode.Create))
        //    {
        //        byte[] archiveHeaderBytes = archiveHeader.GetArchiveHeaderAsBytes();
        //        archiveFileStream.Write(archiveHeaderBytes, 0, archiveHeaderBytes.Length);

        //        if (this.IsRleCompressionActive == true)
        //        {
        //            ///TODO RLE COMPRESSION
        //            /////file headers need to be adjusted after they are wirtten to include the true filesizewithCOMPRESSION!!!
        //            throw new NotImplementedException();
        //        }
        //        else
        //        {
        //            foreach (var fileInfo in filesToBeWrittenIntoArchive)
        //            {
        //                // get the file header as a byte array and write it into the file
        //                byte[] fileHeaderBytes = new IndividualFileHeaderInformation(fileInfo.Name, fileInfo.RelativePathForArchive, fileInfo.Length, fileInfo.Length).GetFileHeaderAsByteArray();
        //                archiveFileStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);

        //                using (var originalFileFileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
        //                {
        //                    int standartBufferLength = 1048576; // 1MB buffer
        //                    var buffer = new byte[standartBufferLength];

        //                    ///TODO look closer as how this actually functions.
        //                    int bytesRead;

        //                    //todo remove this variable
        //                    long mbsRead = 0;
        //                    while ((bytesRead = originalFileFileStream.Read(buffer, 0, buffer.Length)) > 0)
        //                    {
        //                        mbsRead++;
        //                        archiveFileStream.Write(buffer, 0, bytesRead);
        //                        ////////////////////////REMOVE/////////////////////
        //                        if (mbsRead % 100 == 0)
        //                        {
        //                            Console.WriteLine("read mbs already: " + mbsRead);
        //                        }
        //                        ////////////////////////////////////////////////////
        //                    }

        //                    //is it  even necessary to close the streams when im ALREADY use using?
        //                    originalFileFileStream.Close();
        //                }
        //            }
        //            archiveFileStream.Close();
        //        }
        //    }
        //}

        public void AppendToArchive(string archiveFilePath, List<FileMetaInformation> filesToBeWrittenIntoArchive,ArchiveHeader newModifiedArchiveHeader)
        {
            long[] expectedFileSizes = this.ReturnCompressedSizeForFilesAsArray(filesToBeWrittenIntoArchive);

            if (!this.CheckExpectedFileSizeForAppend(filesToBeWrittenIntoArchive,expectedFileSizes))
            {
                //TODO ERRORCODE 1
                throw new InvalidOperationException("Not enough diskspace to create the archive!");
            }

           

            // one could even make a method that just appends a lot of files
            for (int i = 0; i < filesToBeWrittenIntoArchive.Count; i++)
            {
                FileMetaInformation fileInfo = filesToBeWrittenIntoArchive[i];
                this.AppendFileWithFileHeaderToArchive(archiveFilePath, fileInfo,expectedFileSizes[i]);
            }


            this.ChangeArchiveHeaderToNewHeader(archiveFilePath, newModifiedArchiveHeader);
            
        }

        private void ChangeArchiveHeaderToNewHeader(string archiveFilePath, ArchiveHeader newModifiedArchiveHeader)
        {
            byte[] newArchiveHeaderAsBytes = newModifiedArchiveHeader.GetArchiveHeaderAsBytes();

            //Just overwrite the old ArchiveHeader
            using (FileStream fs = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Write))
            {
                fs.Write(newArchiveHeaderAsBytes, 0, newArchiveHeaderAsBytes.Length);
            }

        }

        public void CreateArchive(ArchiveHeader archiveHeader, List<FileMetaInformation> filesToBeWrittenIntoArchive)
        {
            string archiveFilePath = Path.Combine(DestinationFolder, ArchiveName);

            long[] expectedFileSizes = this.ReturnCompressedSizeForFilesAsArray(filesToBeWrittenIntoArchive);
            //CHECKING FOR DISK SPACE ON THE DISK that houses the desired archive directory
            if (!this.CheckExpectedFileSizeForAppend(filesToBeWrittenIntoArchive,expectedFileSizes))
            {
                //TODO ERRORCODE 1
                throw new InvalidOperationException("Not enough diskspace to create the archive!");
            }

            // creating a new file or overwritting the old one with this filemode

            ///TODO !!!!! TRY CATCH SO THAT WHEN SOMEBODY ELSE USES THE FILE IT DOESNT THROW SHIT LIKE AN APE
            ///

            //write the archive header to the file
            this.WriteArchiveHeaderToFile(archiveFilePath, archiveHeader);

            for (int i = 0; i < filesToBeWrittenIntoArchive.Count; i++)
            {
                FileMetaInformation fileInfo = filesToBeWrittenIntoArchive[i];
                this.AppendFileWithFileHeaderToArchive(archiveFilePath, fileInfo,expectedFileSizes[i]);
                //is it  even necessary to close the streams when im ALREADY use using?
            }
            /////////////////////////////////// REVISE THE ARCHIVE HEADER AFTER ALL FILES HAVE BEEN READ
        }

        private void AppendFileWithFileHeaderToArchive(string archiveFilePath, FileMetaInformation fileInfo,long compressedSizeOfFile)
        {
            this.WriteFileHeaderToArchive(archiveFilePath, fileInfo,compressedSizeOfFile);

            this.CompressionAlgorithm.Compress(fileInfo.FullName, archiveFilePath);

            //////////////////////////////////REVISE THE FILEHEADER TO CONTAIN ACCURATE INFORMATION ON THE FILE
        }

        private void WriteFileHeaderToArchive(string archiveFilePath, FileMetaInformation fileInfo, long compressedFileSize)
        {
            // get the file header as a byte array and write it into the file
            byte[] fileHeaderBytes = new IndividualFileHeaderInformation(fileInfo.Name, fileInfo.RelativePathForArchive, fileInfo.Length, compressedFileSize).GetFileHeaderAsByteArray();
            using (var archiveFileStream = new FileStream(archiveFilePath, FileMode.Append))
            {
                archiveFileStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
            }
        }

        private void WriteArchiveHeaderToFile(string archiveFilePath, ArchiveHeader archiveHeader)
        {
            //TODO CHECK IF FILE ALREADY EXISTS MAYBE?
            using (var archiveFileStream = new FileStream(archiveFilePath, FileMode.Create))
            {
                byte[] archiveHeaderBytes = archiveHeader.GetArchiveHeaderAsBytes();
                archiveFileStream.Write(archiveHeaderBytes, 0, archiveHeaderBytes.Length);
            }
        }

        //returns true if the destination folder contains enough space for the compression.
        private bool CheckExpectedFileSizeForAppend(List<FileMetaInformation> fileMetaInformationList,long[] expectedSizesForFiles)
        {
            long sumExpectedFileSize = 0;

            //add the archiveheadersize

            long counter = 0;
            sumExpectedFileSize += new FixedVariables().ArchiveHeaderLength;
            foreach (var item in fileMetaInformationList)
            {
                //get the length of each individual file header
                IndividualFileHeaderInformation header = new IndividualFileHeaderInformation(item.Name, item.RelativePathForArchive, item.Length, item.Length);
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

        private long[] ReturnCompressedSizeForFilesAsArray(List<FileMetaInformation> fileMetaInformationList)
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