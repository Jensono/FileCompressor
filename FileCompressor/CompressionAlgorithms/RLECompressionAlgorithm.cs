using System;
using System.IO;
using System.Linq;

namespace FileCompressor
{
    internal class RLECompressionAlgorithm : ICompressionAlgorithm
    {
        public RLECompressionAlgorithm()
        {
        }

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
                        //TODO REVERT BACK TO 1048576; // 1MB buffer!
                        int standartBufferLength = 1048576; // 1MB buffer
                        var rawBuffer = new byte[standartBufferLength];

                        ///TODO look closer as how this actually functions.
                        int bytesRead;

                        byte transferByteFromLastLoop = 0;
                        byte numberOfOccuranceThatByteLastLoop = 0;

                        while ((bytesRead = originalFileFileStream.Read(rawBuffer, 0, rawBuffer.Length)) > 0)
                        {
                            //trim the bytearray to the number of bytes read
                            // loop throught the array one by one, if the current byte is the same as the last one count it until 256, then start counting again.

                            byte[] realBuffer = rawBuffer.Take(bytesRead).ToArray();

                            //starts at 0 but already signals one occurance
                            byte currentByteOccuranceCounter = numberOfOccuranceThatByteLastLoop;
                            //i need to intialize it or else i get an error
                            byte lastReadByte = transferByteFromLastLoop;
                            for (int i = 0; i < realBuffer.Length; i++)
                            {
                                //if it is the first byte read ever in the array just note what byte it was
                                if (i == 0 && numberOfOccuranceThatByteLastLoop == 0)
                                {
                                    lastReadByte = realBuffer[i];
                                    currentByteOccuranceCounter++;
                                }
                                else
                                {
                                    //if the byte is the same as the last one add one to the counter
                                    if (realBuffer[i] == lastReadByte && currentByteOccuranceCounter != 255)
                                    {
                                        currentByteOccuranceCounter++;
                                    }
                                    //else if there already are 255 occurances of the byte write it into the file and start the counting a new  or start a new count for the new byte value.
                                    else
                                    {
                                        byte[] compressionTwoBytes = new byte[2];
                                        compressionTwoBytes[0] = currentByteOccuranceCounter;
                                        compressionTwoBytes[1] = lastReadByte;
                                        archiveFileStream.Write(compressionTwoBytes, 0, 2);

                                        lastReadByte = realBuffer[i];
                                        currentByteOccuranceCounter = 1;
                                    }
                                }

                                //if it is the last byte in the bytearray just take it to the next loop
                                if (i == realBuffer.Length - 1)
                                {
                                    transferByteFromLastLoop = lastReadByte;
                                    numberOfOccuranceThatByteLastLoop = currentByteOccuranceCounter;
                                    //transfer the lastreadbyte and its occurances to the next loop cycle
                                    //if it is the last loop cycle just commit to the file
                                }
                            }
                        }
                        //if there still is some leftover bytes that never were read, commit them to the file
                        if (numberOfOccuranceThatByteLastLoop != 0)
                        {
                            byte[] compressionTwoBytes = new byte[2];
                            compressionTwoBytes[0] = numberOfOccuranceThatByteLastLoop;
                            compressionTwoBytes[1] = transferByteFromLastLoop;
                            archiveFileStream.Write(compressionTwoBytes, 0, 2);
                        }
                    }
                }
            }
            //TODO specify exceptions
            catch (Exception e)
            {
                ////    catch (IOException e)
                ////{
                ////    // Handle I/O errors.
                ////}
                ////catch (UnauthorizedAccessException e)
                ////{
                ////    // Handle permission errors.
                ////}
                ////catch (ArgumentException e)
                ////{
                ////    // Handle invalid argument errors.
                ////}
                ////catch (PathTooLongException e)
                ////{
                ////    // Handle Path Too Long errors.
                ////}
                ////catch (DirectoryNotFoundException e)
                ////{
                ////    // Handle directory not found errors.
                ////}
                ////catch (NotSupportedException e)
                ////{
                ////    // Handle Not Supported errors.
                ////}
                ////catch (FileNotFoundException e)
                ////{
                ////    // Handle file not found errors.
                ////}
                ////catch (SecurityException e)
                ////{
                ////    // Handle security errors.
                ////}
                ////catch (Exception e)
                ////{
                ////    // Handle any other exception
                ////    throw;
                ////}

                throw e;
            }
        }

        public string CompressionTypeCalling()
        {
            return new FixedVariables().CompressionCallingTypeRLECompression;
        }

        public void Decompress(FileStream archiveFileStream, string outputNewFilePath, long decompressionStartIndexInFile, IndividualFileHeaderInformation fileHeader)
        {
            // Writting the new file here.
            int standartBufferLength = 1024;
            byte[] buffer = new byte[standartBufferLength];

            //create a new file with the output path.
            using (FileStream extratedNewFileStream = new FileStream(outputNewFilePath, FileMode.Create, FileAccess.Write))
            {
                //IF ERRORS CHANGE THIS ; PRB THE CULPRIT
                archiveFileStream.Seek(decompressionStartIndexInFile, SeekOrigin.Begin);

                // we read as long as we have found out we need to from the fileheader
                long bytesLeft = fileHeader.SizeCompressed;
                //read the archive contents in kilobit chunks and only start reading with less when nearing the end. Eg less than the usual buffer is left.

                while (bytesLeft > standartBufferLength)
                {
                    archiveFileStream.Read(buffer, 0, buffer.Length);
                    //going forward every two bytes as the rle compression always consists of 2 bytes, the first one as the counter and the second one as the actual byte that was saved
                    for (int i = 0; i < buffer.Length; i += 2)
                    {
                        byte[] bytesToWriteBuffer = Enumerable.Repeat(buffer[i + 1], buffer[i]).ToArray();
                        extratedNewFileStream.Write(bytesToWriteBuffer, 0, bytesToWriteBuffer.Length);
                    }

                    bytesLeft -= buffer.Length;
                }
                //read the last remaining bits before the filecontent ends in the archive.
                byte[] lastBuffer = new byte[bytesLeft];
                archiveFileStream.Read(lastBuffer, 0, lastBuffer.Length);

                for (int i = 0; i < lastBuffer.Length; i += 2)
                {
                    byte[] bytesToWriteBuffer = Enumerable.Repeat(lastBuffer[i + 1], lastBuffer[i]).ToArray();
                    extratedNewFileStream.Write(bytesToWriteBuffer, 0, bytesToWriteBuffer.Length);
                }
            }
        }

        public long ReturnExpectedDataSizeCompressed(string inputOriginalFilePath)
        {
            long expectedFileSize = 0;
            try
            {
                using (var originalFileFileStream = new FileStream(inputOriginalFilePath, FileMode.Open, FileAccess.Read))
                {
                    int standartBufferLength = 1048576; // 1MB buffer
                    var rawBuffer = new byte[standartBufferLength];

                    ///TODO look closer as how this actually functions.
                    int bytesRead;

                    byte transferByteFromLastLoop = 0;
                    byte numberOfOccuranceThatByteLastLoop = 0;

                    while ((bytesRead = originalFileFileStream.Read(rawBuffer, 0, rawBuffer.Length)) > 0)
                    {
                        //trim the bytearray to the number of bytes read
                        // loop throught the array one by one, if the current byte is the same as the last one count it until 256, then start counting again.

                        byte[] realBuffer = rawBuffer.Take(bytesRead).ToArray();

                        //starts at 0 but already signals one occurance
                        byte currentByteOccuranceCounter = numberOfOccuranceThatByteLastLoop;

                        //i need to intialize it or else i get an error
                        byte lastReadByte = transferByteFromLastLoop;
                        for (int i = 0; i < realBuffer.Length; i++)
                        {
                            //if it is the first byte read ever in the array just note what byte it was
                            if (i == 0 && numberOfOccuranceThatByteLastLoop == 0)
                            {
                                lastReadByte = realBuffer[i];
                                currentByteOccuranceCounter++;
                            }
                            else
                            {
                                //if the byte is the same as the last one add one to the counter
                                if (realBuffer[i] == lastReadByte && currentByteOccuranceCounter != 255)
                                {
                                    currentByteOccuranceCounter++;
                                }
                                //else if there already are 255 occurances of the byte write it into the file and start the counting a new  or start a new count for the new byte value.
                                else
                                {
                                    expectedFileSize += 2;

                                    lastReadByte = realBuffer[i];
                                    currentByteOccuranceCounter = 1;
                                }
                            }

                            //if it is the last byte in the bytearray just take it to the next loop
                            if (i == realBuffer.Length - 1)
                            {
                                transferByteFromLastLoop = lastReadByte;
                                numberOfOccuranceThatByteLastLoop = currentByteOccuranceCounter;
                                //transfer the lastreadbyte and its occurances to the next loop cycle
                                //if it is the last loop cycle just commit to the file
                            }
                        }
                    }
                    //if there still is some leftover bytes that never were read, commit them to the file
                    if (numberOfOccuranceThatByteLastLoop != 0)
                    {
                        expectedFileSize += 2;
                    }
                }
            }
            //TODO specify exceptions
            catch (Exception e)
            {
                throw e;
            }
            return expectedFileSize;
        }
    }
}