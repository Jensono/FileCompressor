using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCompressor
{
    //TODO FILEHEADER MUST MUST MUST BE CHECKED OR WEIRD ASS SHIT IS GOING TO HAPPEN, excpetions like arithemeticoperationinvalid already seen in here.
    internal class ArchiveFileReader
    {
        //TODO PROPERTIES
        public string ArchiveSource { get; set; }
        public FixedVariables FixedVariables { get; set; }

        

        public ICompressionAlgorithm CompressionAlogrithmenUsed { get; private set; }

        public ArchiveFileReader(string source)
        {
            ArchiveSource = source;
            this.FixedVariables = new FixedVariables();

            this.CompressionAlogrithmenUsed = new FixedVariables().GetCompressionAlgorithmFromCalling(this.ReturnArchiveHeader().CompressionTypeCalling);


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
                throw new Exception("CHANGE THIS SHIT TODO");
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
                        IndividualFileHeaderInformation fileHeader = this.ReadIndividualFileHeader(filestream, currentPositionInFile);
                        //skip the file , we dont need that shit, we only need the information
                        filestream.Seek(fileHeader.SizeCompressed, SeekOrigin.Current);

                        foundFileInformationList.Add(fileHeader);
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
                foundFileNames.Add(item.Name);
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
            int numberOfFilesInArchive = 0;
            long sizeOfDecompressedFiles = 0;
            ArchiveHeader header;

            if (!this.IsArchiveHeaderValid(out header))
            {
                throw new InvalidOperationException("header is corrupted or used invalid file!");
                //this method must throw a exception, todo i dont know if i need to catch it here.
            }

            numberOfFilesInArchive = header.NumberOfFilesInArchive;
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
                    long currentPositionInFile = this.FixedVariables.ArchiveHeaderLength;

                    for (int i = 0; i < numberOfFilesInArchive; i++)
                    {
                        IndividualFileHeaderInformation fileHeader = this.ReadIndividualFileHeader(archiveFilestream, currentPositionInFile);

                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        // TODO Repeat this for all files inside the archive.
                        //add a failsave that is triggered if there are fewer files found in the archive than expected or if there are more.
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ///

                        // first we need to create a directory for the new directory , combinign the relative path with the given source
                        string outputPath = Path.Combine(destination, fileHeader.RelativePath);

                        //DOES 
                        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                        currentPositionInFile = archiveFilestream.Position;

                        this.CompressionAlogrithmenUsed.Decompress(archiveFilestream, outputPath,currentPositionInFile, fileHeader);
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

        //Does and should not need a compression algorithm as all data is stored the same way in the archive header, is used to also find out which compression was used and rectify the class itself!.
        public ArchiveHeader ReturnArchiveHeader()
        {
            ArchiveHeader header;
            if (this.IsArchiveHeaderValid(out header))
            {
                return header;
            }
            else
            {
                Console.WriteLine("given filepath was invalid cant generate ArchiveHeader");
                throw new Exception("some shit happend here dat should not have happend.");
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

        public IndividualFileHeaderInformation ReadIndividualFileHeader(FileStream archiveFilestream, long currentPositionInFile)
        {
            //TODO EXCEPTIONS, maybe try catch

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

            return new IndividualFileHeaderInformation(fileName, filePath, fileSizeNoCompression, fileSizeWithCompression);
        }
    }
}