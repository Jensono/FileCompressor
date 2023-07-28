//-----------------------------------------------------------------------
// <copyright file="ArchiveFileReader.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is used to read files, validate that they are archive files and perform operation on them like appending them or reading out information contained within.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// This class is used to read files, validate that they are archive files and perform operation on them like appending them or reading out information contained within.
    /// </summary>
    public class ArchiveFileReader
    {
        /// <summary>
        /// The field for the source path of the archive.
        /// </summary>
        private string archiveSource;

        /// <summary>
        /// The field for the FixedVariables class used inside the File reader.
        /// </summary>
        private FixedVariables fixedVariables;

        /// <summary>
        /// The field for the Compression Algorithm that is used in the archive.
        /// </summary>
        private ICompressionAlgorithm compressionAlgorithmUsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveFileReader"/> class.
        /// </summary>
        /// <param name="source"> The path to the Archive as a string.</param>
        public ArchiveFileReader(string source)
        {
            this.ArchiveSource = source;
            this.FixedVariables = new FixedVariables();
            ICompressionAlgorithm compressionUsed = new FixedVariables().GetCompressionAlgorithmFromCalling(this.ReturnArchiveHeader().CompressionTypeCalling);
            if (compressionUsed is null)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Given archive source {source} does not contain a valid Compression! ");
            }

            this.CompressionAlogrithmenUsed = compressionUsed;
        }

        /// <summary>
        /// Gets or sets the Source to the archive as a path.
        /// </summary>
        /// <value> The Source to the archive as a path. </value>
        public string ArchiveSource
        {
            get
            {
                return this.archiveSource;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ArchiveSource)} can not be null!");
                }

                this.archiveSource = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="FixedVariables"/> class used inside the reader.
        /// </summary>
        /// <value> A fixed variable object. </value>
        public FixedVariables FixedVariables
        {
            get
            {
                return this.fixedVariables;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.FixedVariables)} cannot be null!");
                }

                this.fixedVariables = value;
            }
        }


        /// <summary>
        /// Gets the <see cref="ICompressionAlgorithm"/> Compression Algorithm that is used in the archive.
        /// </summary>
        /// <value> The <see cref="ICompressionAlgorithm"/> Compression Algorithm that is used in the archive.  </value>
        public ICompressionAlgorithm CompressionAlogrithmenUsed
        {
            get
            {
                return this.compressionAlgorithmUsed;
            }

            private set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.CompressionAlogrithmenUsed)} cannot be null!");
                }

                this.compressionAlgorithmUsed = value;
            }
        }

        /// <summary>
        /// This method reads the entity of the archive and returns a list of strings, containing the file names that were found in the header.
        /// </summary>
        /// <returns> A list of string, in which each entry is a filename for a file that is contained inside the archive.</returns>
        public List<string> ReadArchiveFileAndReturnEntries()
        {
            List<string> foundFileNames = new List<string>();

            byte[] initialBuffer = new byte[this.FixedVariables.ArchiveHeaderLength];

            // First try and check if the archive header is build normally, from the archive header we only need the number of items that is saved in the file.
            int numberOfFilesInFile = 0;
            ArchiveHeader header;

            try
            {
                this.IsArchiveHeaderValid(out header);
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }

            numberOfFilesInFile = header.NumberOfFilesInArchive;

            List<IndividualFileHeaderInformation> foundFileInformationList = new List<IndividualFileHeaderInformation>();

            try
            {
                using (FileStream filestream = new FileStream(this.ArchiveSource, FileMode.Open, FileAccess.Read))
                {
                    // by defoult we start where the archive header ends.
                    long currentPositionInFile = this.FixedVariables.ArchiveHeaderLength;

                    for (int i = 0; i < numberOfFilesInFile; i++)
                    {
                        IndividualFileHeaderInformation fileHeader = this.ReadIndividualFileHeader(filestream, currentPositionInFile);

                        // skip the file , we dont need that shit, we only need the information
                        filestream.Seek(fileHeader.SizeCompressed, SeekOrigin.Current);

                        foundFileInformationList.Add(fileHeader);
                        currentPositionInFile = filestream.Position;
                    }
                }
            }
            catch (ArchiveErrorCodeException e)
            {
                e.AppendErrorCodeInformation($" Filepath: {this.ArchiveSource}");
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

            foreach (var item in foundFileInformationList)
            {
                foundFileNames.Add(item.Name);
            }

            return foundFileNames;
        }

        // destination needs to be a valid folder, also the drive for the ditrectory needs to have enough space to hold the files after extraction.

        /// <summary>
        /// This method takes a destination path and tries to execute the archives files found inside the archive.
        /// </summary>
        /// <param name="destination"> The destination folder into which to extract the found files.</param>
        /// <exception cref="ArchiveErrorCodeException"> 
        /// Is raised when: The Archive header is presumed corrupted.
        ///                 A individual file header is presumed corrupted.
        ///                 There is not enough disk space for the extracted files on the disk specified by the destination path.
        ///                 If there already is an archive with the same name that is set to read only.
        ///                 If a <see cref="OutOfMemoryException"/> occurs due to a corrupt file.                 
        /// </exception>
        public void ExtractArchiveFiles(string destination)
        {
            List<string> foundFileNames = new List<string>();

            // First try and check if the archive header is build normally, from the archive header we only need the number of items that is saved in the file.
            // We also need to check for enough disk space for when extracting.
            int numberOfFilesInArchive = 0;
            long sizeOfDecompressedFiles = 0;
            ArchiveHeader header;

            if (!this.IsArchiveHeaderValid(out header))
            {
                throw new ArchiveErrorCodeException("Errorcode 1. Archive is corrupted or used invalid file!");
            }

            numberOfFilesInArchive = header.NumberOfFilesInArchive;
            sizeOfDecompressedFiles = header.SizeOfFilesCombined;

            // this class alone already checks if a given directory is valid
            DirectorySourceProcessor directorySourceProcessor = new DirectorySourceProcessor(destination);
            try
            {
                directorySourceProcessor.CheckForEnoughDriveSpace(sizeOfDecompressedFiles);
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }

            List<IndividualFileHeaderInformation> foundFileInformationList = new List<IndividualFileHeaderInformation>();

            try
            {
                using (FileStream archiveFilestream = new FileStream(this.ArchiveSource, FileMode.Open, FileAccess.Read))
                {
                    // by defoult we start where the archive header ends.
                    long currentPositionInFile = this.FixedVariables.ArchiveHeaderLength;

                    for (int i = 0; i < numberOfFilesInArchive; i++)
                    {
                        IndividualFileHeaderInformation fileHeader = this.ReadIndividualFileHeader(archiveFilestream, currentPositionInFile);

                        // first we need to create a directory for the new directory , combinign the relative path with the given source
                        string outputPath = Path.Combine(destination, fileHeader.RelativePath);

                        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                        currentPositionInFile = archiveFilestream.Position;

                        this.CompressionAlogrithmenUsed.Decompress(archiveFilestream, outputPath, currentPositionInFile, fileHeader);

                        // set the position in the file to the last position read.
                        currentPositionInFile = archiveFilestream.Position;
                    }
                }
            }
            catch (ArchiveErrorCodeException e)
            {
                e.AppendErrorCodeInformation($" Filepath: {this.ArchiveSource}");
                throw e;
            }
            catch (UnauthorizedAccessException e)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Could not access file with Path: {this.ArchiveSource}");
            }           
            catch (OutOfMemoryException e)
            { // had this exception once when i was messing with the individual file headers, should be fixed now with the archiverror that is given when the filename or the path is too long.
                throw new ArchiveErrorCodeException($"Errorcode 1. Archive at {this.ArchiveSource} is possibly corrupted");
            }
            catch (Exception e)
            {
                // TODO REMOVE THIS SHIT
                throw e;
            }
        }

        /// <summary>
        /// This method tries to read the Archive header from the source path and print the meta information about the archive to the console.
        /// </summary>
        /// <exception cref="ArchiveErrorCodeException"> Is raised when the archive header could not be read.</exception>
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
                throw new ArchiveErrorCodeException("Errorcode 1. File corrupted or file isn't a archive file! ");
            }
        }

        // Does and should not need a compression algorithm as all data is stored the same way in the archive header, is used to also find out which compression was used and rectify the class itself!.

        /// <summary>
        /// This method reads the archive header with the given source path inside the class and returns a ArchiveHeader if one was found.
        /// </summary>
        /// <returns> A valid Archive header if one was found, else it returns null.</returns>
        /// <exception cref="ArchiveErrorCodeException">
        /// Is raised if: a ArchiveError occurred while processing.
        ///               The file was read only and could not be accessed.
        /// </exception>
        public ArchiveHeader ReturnArchiveHeader()
        {
            ArchiveHeader header;
            bool wasArchiveHeaderReadable;
            try
            {
                wasArchiveHeaderReadable = this.IsArchiveHeaderValid(out header);
            }
            catch (ArchiveErrorCodeException e)
            {
                e.AppendErrorCodeInformation($"Source File: {this.ArchiveSource} ");
                throw e;
            }           
            catch (UnauthorizedAccessException e)
            { // can happen if somebody sets a folder for extraction
                throw new ArchiveErrorCodeException("Errorcode 1. Could not acess given file." + $"Source File: {this.ArchiveSource} ");
            }
            catch (Exception e)
            {
                throw e;
            }

            if (wasArchiveHeaderReadable)
            {
                return header;
            }
            else
            {
                // THIS SHOULD NEVER EVER HAPPEN TODO
                return null;
            }
        }

        

        public IndividualFileHeaderInformation ReadIndividualFileHeader(FileStream archiveFilestream, long currentPositionInFile)
        {
            if (archiveFilestream is null)
            {
                throw new ArgumentNullException($"{nameof(archiveFilestream)} can not be null!");
            }

            byte[] stringNameSizeBuffer = new byte[4];

            // Skip the first 21 Bytes as these are the Archive Header
            archiveFilestream.Seek(currentPositionInFile, SeekOrigin.Begin);

            // read from the filestream the length of the filename
            archiveFilestream.Read(stringNameSizeBuffer, 0, stringNameSizeBuffer.Length);

            // Convert the size to int
            int sizeOfNameInBytes = BitConverter.ToInt32(stringNameSizeBuffer, 0);

            // length of the name and path cant be bigger than a certain value and also will never be zero!
            if (sizeOfNameInBytes > this.FixedVariables.AbsoluteLimitBytesForFileNameAndPath || sizeOfNameInBytes < 0)
            {
                throw new ArchiveErrorCodeException("Errorcode 1. Archive File is possibly corrupted");
            }

            byte[] fileNameBuffer = new byte[sizeOfNameInBytes];

            // get the file name from the fileheader
            archiveFilestream.Read(fileNameBuffer, 0, fileNameBuffer.Length);
            string fileName = Encoding.UTF8.GetString(fileNameBuffer);

            // Read the size of the pathname
            byte[] stringPathSizeBuffer = new byte[4];
            archiveFilestream.Read(stringPathSizeBuffer, 0, stringPathSizeBuffer.Length);
            int sizeOfPathInBytes = BitConverter.ToInt32(stringPathSizeBuffer, 0);
            if (sizeOfPathInBytes > this.FixedVariables.AbsoluteLimitBytesForFileNameAndPath || sizeOfNameInBytes < 0)
            {
                throw new ArchiveErrorCodeException("Errorcode 1. Archive File is possibly corrupted");
            }

            byte[] filePathBuffer = new byte[sizeOfPathInBytes];

            // get the path name from the fileheader
            archiveFilestream.Read(filePathBuffer, 0, filePathBuffer.Length);
            string filePath = Encoding.UTF8.GetString(filePathBuffer);

            byte[] fileSizeNoCompressionBuffer = new byte[8];

            archiveFilestream.Read(fileSizeNoCompressionBuffer, 0, fileSizeNoCompressionBuffer.Length);
            long fileSizeNoCompression = BitConverter.ToInt64(fileSizeNoCompressionBuffer, 0);

            byte[] fileSizeWithCompressionBuffer = new byte[8];
            archiveFilestream.Read(fileSizeWithCompressionBuffer, 0, fileSizeWithCompressionBuffer.Length);
            long fileSizeWithCompression = BitConverter.ToInt64(fileSizeWithCompressionBuffer, 0);
            IndividualFileHeaderInformation individualFileHeader;

            try
            {
                individualFileHeader = new IndividualFileHeaderInformation(fileName, filePath, fileSizeNoCompression, fileSizeWithCompression);
            }
            catch (ArgumentNullException)
            {
                throw new ArchiveErrorCodeException("Errorcode 1. Given Source may be corrupted! ");
            }
            catch (ArgumentException)
            {
                throw new ArchiveErrorCodeException("Errorcode 1. Given Source may be corrupted! ");
            }

            return individualFileHeader;
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
            catch (UnauthorizedAccessException e)
            {
                throw new ArchiveErrorCodeException($"Errocode 1. Given FileName could not be accesed: {this.ArchiveSource}");
            }
            catch (FileNotFoundException e)
            {
                throw new ArchiveErrorCodeException($"Errocode 1. Given FileName does not exist : {this.ArchiveSource}");
            }
            catch (Exception e)
            {
                throw e;
            }

            try
            {
                ArchiveHeader foundArchiveHeader = new ArchiveHeader(buffer);
                header = foundArchiveHeader;
                return true;
            }
            catch (ArgumentNullException e)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1.The source file {this.ArchiveSource} did not contain a valid header");
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1.The source file {this.ArchiveSource} did not contain a valid header");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}