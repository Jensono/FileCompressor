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
                    /////file headers need to be adjusted after they are wirtten to include the true filesizewithCOMPRESSION!!!
                    throw new NotImplementedException();
                }
                else
                {
                    foreach (var fileInfo in filesToBeWrittenIntoArchive)
                    {
                        // get the file header as a byte array and write it into the file
                        byte[] fileHeaderBytes = new IndividualFileHeaderInformation(fileInfo.Name, fileInfo.RelativePathForArchive, fileInfo.Length, fileInfo.Length).GetFileHeaderAsByteArray();
                        archiveFileStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);


                        using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                        {

                            int standartBufferLength = 1048576; // 1MB buffer
                            var buffer = new byte[standartBufferLength]; 

                            ///TODO look closer as how this actually functions.
                            int bytesRead;

                            //todo remove this variable
                            long mbsRead = 0;
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                mbsRead++;
                                archiveFileStream.Write(buffer, 0, bytesRead);
                                ////////////////////////REMOVE/////////////////////
                                if (mbsRead%100 ==0)
                                {
                                    Console.WriteLine("read mbs already: " + mbsRead);
                                }
                                ////////////////////////////////////////////////////
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