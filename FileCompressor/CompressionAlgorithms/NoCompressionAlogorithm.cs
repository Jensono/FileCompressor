using System;
using System.IO;

namespace FileCompressor
{
    internal class NoCompressionAlgorithm : ICompressionAlgorithm
    {
        public NoCompressionAlgorithm()
        {
        }

        //In theory a compress is just a append, because right now i dont need to compress at any other place other then
        public void Compress(string inputOriginalFilePath, string outputArchiveFilePath)
        {
            try
            {
                using (var archiveFileStream = new FileStream(outputArchiveFilePath, FileMode.Append))
                {
                    //TODO TRY CATCH
                    //+		$exception	{"Der Prozess kann nicht auf die Datei \"C:\\Users\\Jensh\\Desktop\\Testdatein\\test.jth\" zugreifen, da sie von einem anderen Prozess verwendet wird."}	System.IO.IOException

                    using (var originalFileFileStream = new FileStream(inputOriginalFilePath, FileMode.Open, FileAccess.Read))
                    {
                        int standartBufferLength = 1048576; // 1MB buffer
                        var buffer = new byte[standartBufferLength];

                        ///TODO look closer as how this actually functions.
                        int bytesRead;

                        while ((bytesRead = originalFileFileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            archiveFileStream.Write(buffer, 0, bytesRead);
                        }

                        //is it  even necessary to close the streams when im ALREADY use using?
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

        //create a class or enum or something that holds these values, so they dont lose meaning over time.
        public string ReturnCompressionTypeCalling()
        {
            return "None";
        }

        public void Decompress(FileStream archiveFilestream, string outputNewFilePath, long archiveDecompressionStartPoint, IndividualFileHeaderInformation fileHeader)
        {
            // Writting the new file here.
            int standartBufferLength = 1024;
            byte[] buffer = new byte[standartBufferLength];

            //create a new file with the output path.
            using (FileStream extratedNewFileStream = new FileStream(outputNewFilePath, FileMode.Create, FileAccess.Write))
            {
                //IF ERRORS CHANGE THIS ; PRB THE CULPRIT
                archiveFilestream.Seek(archiveDecompressionStartPoint, SeekOrigin.Begin);

                // we read as long as we have found out we need to from the fileheader
                long bytesLeft = fileHeader.SizeCompressed;
                //read the archive contents in kilobit chunks and only start reading with less when nearing the end. Eg less than the usual buffer is left.

                while (bytesLeft > standartBufferLength)
                {
                    archiveFilestream.Read(buffer, 0, buffer.Length);
                    extratedNewFileStream.Write(buffer, 0, buffer.Length);
                    bytesLeft -= buffer.Length;
                }
                //read the last remaining bits before the filecontent ends in the archive.
                byte[] lastBuffer = new byte[bytesLeft];
                archiveFilestream.Read(lastBuffer, 0, lastBuffer.Length);
                extratedNewFileStream.Write(lastBuffer, 0, lastBuffer.Length);
            }
        }

        public long ReturnExpectedDataSizeCompressed(string inputOriginalFilePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(inputOriginalFilePath);
                return fileInfo.Length;
            }
            //TODO SPECIFIY EXCEPTION
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}