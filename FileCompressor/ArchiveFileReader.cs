using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCompressor
{
    //TODO FILEHEADER MUST MUST MUST BE CHECKED OR WEIRD ASS SHIT IS GOING TO HAPPEN, excpetions like arithemeticoperationinvalid already seen in here.
    internal class ArchiveFileReader
    {
        public string ArchiveSource { get; set; }
        public FixedVariables FixedVariables { get; set; }
        public ArchiveFileReader(string source)
        {
            ArchiveSource = source;
            this.FixedVariables = new FixedVariables();
        }

        public List<string> ReadArchiveFileAndReturnEntries()
        {
            List<string> foundFileNames = new List<string>();

            // TODO TODO TODO !!! make a class that reads an archive, and but all the relevant parts in there: confirming validity, reading header, skipping header,reading file info, extracting file info,

            byte[] initialBuffer = new byte[this.FixedVariables.ArchiveHeaderLength];

            // First try and check if the archive header is build normally, from the archive header we only need the number of items that is saved in the file.
            int numberOfFilesInFile = 0;
            ArchiveHeader header;

            if (!this.IsArchiveHeaderValid(out header))
            {
                //this method must throw a exception, todo i dont know if i need to catch it here.
            }

            numberOfFilesInFile = header.NumberOfFilesInArchive;

            List<IndividualFileHeaderInformation> foundFileInformationList = new List<IndividualFileHeaderInformation>();

            try
            {
                using (FileStream filestream = new FileStream(this.ArchiveSource, FileMode.Open, FileAccess.Read))
                {
                    //by defoult we start where the archive header ends.
                    long currentPositionInFile = this.FixedVariables.ArchiveHeaderLength;

                    for (int i = 0; i < numberOfFilesInFile; i++)
                    {
                        byte[] stringNameSizeBuffer = new byte[4];
                        // Skip the first 21 Bytes as these are the Archive Header
                        filestream.Seek(currentPositionInFile, SeekOrigin.Begin);
                        //read from the filestream the length of the filename
                        filestream.Read(stringNameSizeBuffer, 0, stringNameSizeBuffer.Length);
                        //Convert the size to int

                        //TODO CHECK TO SEE IF POSITIV IF NOT END THE READ!
                        int SizeOfNameInBytes = BitConverter.ToInt32(stringNameSizeBuffer, 0);

                        byte[] fileNameBuffer = new byte[SizeOfNameInBytes];

                        //get the file name from the fileheader
                        filestream.Read(fileNameBuffer, 0, fileNameBuffer.Length);
                        string fileName = Encoding.UTF8.GetString(fileNameBuffer);

                        //Read the size of the pathname
                        byte[] stringPathSizeBuffer = new byte[4];
                        filestream.Read(stringPathSizeBuffer, 0, stringPathSizeBuffer.Length);
                        int sizeOfPathInBytes = BitConverter.ToInt32(stringPathSizeBuffer, 0);
                        byte[] filePathBuffer = new byte[sizeOfPathInBytes];

                        //get the path name from the fileheader
                        filestream.Read(filePathBuffer, 0, filePathBuffer.Length);
                        string filePath = Encoding.UTF8.GetString(fileNameBuffer);

                        byte[] fileSizeNoCompressionBuffer = new byte[8];

                        filestream.Read(fileSizeNoCompressionBuffer, 0, fileSizeNoCompressionBuffer.Length);
                        long fileSizeNoCompression = BitConverter.ToInt64(fileSizeNoCompressionBuffer, 0);

                        byte[] fileSizeWithCompressionBuffer = new byte[8];
                        filestream.Read(fileSizeWithCompressionBuffer, 0, fileSizeWithCompressionBuffer.Length);
                        long fileSizeWithCompression = BitConverter.ToInt64(fileSizeWithCompressionBuffer, 0);

                        //skip the file , we dont need that shit, we only need the information
                        filestream.Seek(fileSizeWithCompression, SeekOrigin.Current);

                        foundFileInformationList.Add(new IndividualFileHeaderInformation(fileName, filePath, fileSizeNoCompression, fileSizeWithCompression));
                        currentPositionInFile = filestream.Position;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            //TODO just return a list of strings
            foreach (var item in foundFileInformationList)
            {
                foundFileNames.Add(item.FileName);
            }

            return foundFileNames;
        }

        //destination needs to be a valid folder, also the drive for the ditrectory needs to have enough space to hold the files after extraction.
        public void ExtractArchiveFiles(string destination)
        {
            List<string> foundFileNames = new List<string>();

            // TODO TODO TODO !!! make a class that reads an archive, and but all the relevant parts in there: confirming validity, reading header, skipping header,reading file info, extracting file info,

            // First try and check if the archive header is build normally, from the archive header we only need the number of items that is saved in the file.
            // We also need to check for enough disk space for when extracting.
            int numberOfFilesInFile = 0;
            long sizeOfDecompressedFiles = 0;
            ArchiveHeader header;

            if (!this.IsArchiveHeaderValid(out header))
            {
                //this method must throw a exception, todo i dont know if i need to catch it here.
            }

            numberOfFilesInFile = header.NumberOfFilesInArchive;
            sizeOfDecompressedFiles = header.SizeOfFilesCombined;

            //this class alone already checks if a given directory is valid
            DirectorySourceProcessor directorySourceProcessor = new DirectorySourceProcessor(destination);
            directorySourceProcessor.CheckForEnoughDriveSpace(sizeOfDecompressedFiles);

            List<IndividualFileHeaderInformation> foundFileInformationList = new List<IndividualFileHeaderInformation>();

            // now start extracting the files , TODO WHAT HAPPENS IF one of the files fileheader is corrupted?
            try
            {
                using (FileStream archiveFilestream = new FileStream(this.ArchiveSource, FileMode.Open, FileAccess.Read))
                {
                    //by defoult we start where the archive header ends.
                    long currentPositionInFile = this.FixedVariables.ArchiveHeaderLength ;

                    for (int i = 0; i < numberOfFilesInFile; i++)
                    {


                        ///TODO TODO TODO remove this part to a new function that just retunrs the content of the fileheader and where to start and end reading.


                                    byte[] stringNameSizeBuffer = new byte[4];
                                    // Skip the first 21 Bytes as these are the Archive Header
                                    archiveFilestream.Seek(currentPositionInFile, SeekOrigin.Begin);
                                    //read from the filestream the length of the filename
                                    archiveFilestream.Read(stringNameSizeBuffer, 0, stringNameSizeBuffer.Length);
                                    //Convert the size to int

                                    //TODO CHECK TO SEE IF POSITIV IF NOT END THE READ!
                                    int SizeOfNameInBytes = BitConverter.ToInt32(stringNameSizeBuffer, 0);

                                    byte[] fileNameBuffer = new byte[SizeOfNameInBytes];

                                    //get the file name from the fileheader
                                    archiveFilestream.Read(fileNameBuffer, 0, fileNameBuffer.Length);
                                    string fileName = Encoding.UTF8.GetString(fileNameBuffer);

                                    //Read the size of the pathname
                                    byte[] stringPathSizeBuffer = new byte[4];
                                    archiveFilestream.Read(stringPathSizeBuffer, 0, stringPathSizeBuffer.Length);
                                    int sizeOfPathInBytes = BitConverter.ToInt32(stringPathSizeBuffer, 0);
                                    byte[] filePathBuffer = new byte[sizeOfPathInBytes];

                                    //get the path name from the fileheader
                                    archiveFilestream.Read(filePathBuffer, 0, filePathBuffer.Length);
                                    string filePath = Encoding.UTF8.GetString(filePathBuffer);

                                    byte[] fileSizeNoCompressionBuffer = new byte[8];

                                    archiveFilestream.Read(fileSizeNoCompressionBuffer, 0, fileSizeNoCompressionBuffer.Length);
                                    long fileSizeNoCompression = BitConverter.ToInt64(fileSizeNoCompressionBuffer, 0);

                                    byte[] fileSizeWithCompressionBuffer = new byte[8];
                                    archiveFilestream.Read(fileSizeWithCompressionBuffer, 0, fileSizeWithCompressionBuffer.Length);
                                    long fileSizeWithCompression = BitConverter.ToInt64(fileSizeWithCompressionBuffer, 0);



                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        // TODO Repeat this for all files inside the archive.
                        //add a failsave that is triggered if there are fewer files found in the archive than expected or if there are more.
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ///


                        // irst we need to create a directory for the new directory , combinign the relative path with the given source

                        //todo check if the created path is correct and then delete this comment:
                        //string outputPath = Path.Combine(destination, filePath, fileName);

                        string outputPath = Path.Combine(destination, filePath);

                        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                        //CHANGE THIS SO 
                        if (header.CompressionTypeCalling =="None")
                        {
                            // Writting the new file here.
                            int standartBufferLength = 1024;
                            byte[] buffer = new byte[standartBufferLength];
                            using (FileStream extratedNewFileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                            {
                                // we read as long as we have found out we need to from the fileheader
                                long bytesLeft = fileSizeWithCompression;
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
                        else
                        {
                            //TODO IMPLEMENT RLE DESERIALISING HERE.
                            throw new NotImplementedException("implement rle Extraction here.");
                        }
                       


                        //set the position in the file to the last position read.
                        currentPositionInFile = archiveFilestream.Position;
                    }





                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ReadArchiveHeaderAndPrintToConsole()
        {
            ArchiveHeader header;
            if (this.IsArchiveHeaderValid(out header))
            {
                FileInfo fileInfo = new FileInfo(this.ArchiveSource);
                header.PrintArchiveHeaderToConsole(fileInfo.Name, fileInfo.DirectoryName);
            }
            else
            {
                Console.WriteLine("given filepath was invalid");
                //TODO errorcode and exception should be here
            }
        }

        private bool IsArchiveHeaderValid(out ArchiveHeader header)
        {
            byte[] buffer = new byte[this.FixedVariables.ArchiveHeaderLength];

            try
            {
                using (FileStream fs = new FileStream(this.ArchiveSource, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(buffer, 0, buffer.Length);
                }
            }
            catch (Exception e)
            {
                //TODO change method to do just this
                //header = null;
                //return false;
                throw e;
            }

            try
            {
                ArchiveHeader foundArchiveHeader = new ArchiveHeader(buffer);
                header = foundArchiveHeader;
                return true;
            }
            //TODO BUILD OWN EXCEPTION FOR ARCHIVE HEADER IF THERE WAS DATA THAT WAS NOT EXPECTED!
            catch (Exception e)
            {
                //header = null;
                //return false;
                throw e;
            }
        }
    }
}