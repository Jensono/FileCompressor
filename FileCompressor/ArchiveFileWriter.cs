using System;
using System.Collections.Generic;
using System.IO;

namespace FileCompressor
{
    internal class ArchiveFileWriter
    {
        //source and destination. Then it writes has a method for c

        private string DestinationFolder;
        private string ArchiveName;
        private bool IsRleCompressionActive;

        public ArchiveFileWriter(string destination, string archiveName, bool isRLEActive)
        {
            this.DestinationFolder = destination;
            this.ArchiveName = archiveName;
            this.IsRleCompressionActive = isRLEActive;
        }

        public void CreateArchive(ArchiveHeader archiveHeader, List<FileMetaInformation> filesToBeWrittenIntoArchive)
        {
            string archiveFilePath = Path.Combine(DestinationFolder, ArchiveName);

            // creating a new file or overwritting the old one with this filemode

            ///TODO !!!!! TRY CATCH SO THAT WHEN SOMEBODY ELSE USES THE FILE IT DOESNT THROW SHIT LIKE AN APE
            using (var archiveFileStream = new FileStream(archiveFilePath, FileMode.Create))
            {
                byte[] archiveHeaderBytes = archiveHeader.GetArchiveHeaderAsBytes();
                archiveFileStream.Write(archiveHeaderBytes, 0, archiveHeaderBytes.Length);

                if (this.IsRleCompressionActive == true)
                {
                    ///TODO RLE COMPRESSION
                    throw new NotImplementedException();
                }
                else
                {
                    foreach (var fileInfo in filesToBeWrittenIntoArchive)
                    {
                        // get the file header as a byte array and write it into the file
                        byte[] fileHeaderBytes = new IndividualFileHeaderInformation(fileInfo.FileInfo.Name, fileInfo.RelativePathForArchive, fileInfo.FileInfo.Length).GetFileHeaderAsByteArray();
                        archiveFileStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);


                        using (var fileStream = new FileStream(fileInfo.FileInfo.FullName, FileMode.Open, FileAccess.Read))
                        {
                            var buffer = new byte[1024]; // 1KB buffer

                            ///TODO look closer as how this actually functions.
                            int bytesRead;

                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                archiveFileStream.Write(buffer, 0, bytesRead);
                            }

                            //is it  even necessary to close the streams when im ALREADY use using?
                            fileStream.Close();
                        }

                    }
                    archiveFileStream.Close();
                }
            }
        }

        public void AppendArchive()
        {
            throw new NotImplementedException("SHIT");
        }
    }
}