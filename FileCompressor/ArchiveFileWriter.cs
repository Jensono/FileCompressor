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

        public void AppendToArchive(string archiveFilePath , List<FileMetaInformation> filesToBeWrittenIntoArchive)
        {
            // one could even make a method that just appends a lot of files
            foreach (var fileInfo in filesToBeWrittenIntoArchive)
            {
                this.AppendFileWithFileHeaderToArchive(archiveFilePath, fileInfo);
            }

        }

        public void CreateArchive(ArchiveHeader archiveHeader, List<FileMetaInformation> filesToBeWrittenIntoArchive)
        {
            string archiveFilePath = Path.Combine(DestinationFolder, ArchiveName);

            // creating a new file or overwritting the old one with this filemode

            ///TODO !!!!! TRY CATCH SO THAT WHEN SOMEBODY ELSE USES THE FILE IT DOESNT THROW SHIT LIKE AN APE
            ///

            //write the archive header to the file
            this.WriteArchiveHeaderToFile(archiveFilePath, archiveHeader);

            foreach (var fileInfo in filesToBeWrittenIntoArchive)
            {
                this.AppendFileWithFileHeaderToArchive(archiveFilePath,fileInfo);
                //is it  even necessary to close the streams when im ALREADY use using?
               
            }
            /////////////////////////////////// REVISE THE ARCHIVE HEADER AFTER ALL FILES HAVE BEEN READ
           
        }

        private void AppendFileWithFileHeaderToArchive(string archiveFilePath, FileMetaInformation fileInfo) 
        {
            this.WriteFileHeaderToArchive(archiveFilePath, fileInfo);

            this.CompressionAlgorithm.Compress(fileInfo.FullName, archiveFilePath);

            //////////////////////////////////REVISE THE FILEHEADER TO CONTAIN ACCURATE INFORMATION ON THE FILE
        }

        private void WriteFileHeaderToArchive(string archiveFilePath, FileMetaInformation fileInfo)
        {
            // get the file header as a byte array and write it into the file
            byte[] fileHeaderBytes = new IndividualFileHeaderInformation(fileInfo.Name, fileInfo.RelativePathForArchive, fileInfo.Length, fileInfo.Length).GetFileHeaderAsByteArray();
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
        
    }
}