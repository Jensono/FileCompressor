//-----------------------------------------------------------------------
// <copyright file="FixedVariables.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class contains information on fixed variables that are used in many diffrent parts of the programm. Is currently used as a "dictonary" to save values that should remain consistend throught the programm but still able
// to be modified if so needed.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class contains information on fixed variables that are used in many different parts of the program.
    /// Is currently used as a "dictionary" to save values that should remain consistent throughout the program but still able
    /// to be modified if so needed.
    /// </summary>
    public class FixedVariables
    {
        /// <summary>
        /// The field for the set archive header length inside the program.
        /// </summary>
        private int archiveHeaderLength;

        /// <summary>
        /// The field for the  archive header length without any parity bytes inside the program.
        /// </summary>
        private int archiveHeaderOriginalLength;

        /// <summary>
        /// The field for the index of the first byte of the date time inside the archive header.
        /// </summary>
        private int archiveHeaderDateTimeStartByteIndex;

        /// <summary>
        /// The field for the index of the first byte of the compression type inside the archive header.
        /// </summary>
        private int archiveHeaderCompressionTypeStartByteIndex;

        /// <summary>
        /// The field for the index of the first byte of the number of files attribute inside the archive header.
        /// </summary>
        private int archiveHeaderNumberOfFilesStartByteIndex;

        /// <summary>
        /// The field for the index of the first byte of the sum of files size attribute inside the archive header.
        /// </summary>
        private int archiveHeaderSumOfFileSizeStartByteIndex;

        /// <summary>
        ///  The field for the number of bytes used for the  length of the compression calling.
        /// </summary>
        private int archiveHeaderLengthOfCompressionCalling;

        /// <summary>
        /// The field for the string of the calling for no compression.
        /// </summary>
        private string compressionCallingTypeNoCompression;

        /// <summary>
        /// The field for the string of the calling for RLE compression.
        /// </summary>
        private string compressionCallingTypeRLECompression;

        /// <summary>
        /// The field for the byte that is used as a filler byte inside the archive header if the compression calling is smaller that the compression calling byte array length.
        /// </summary>
        private byte archiveHeaderCompressionCallingFillerByte;

        /// <summary>
        /// The field for the the absolute limit a file name or path can have inside this application.
        /// </summary>
        private int absoluteLimitBytesForFileNameAndPath;

        /// <summary>
        /// The field for the string that is displayed on the console, when the help command is executed.
        /// </summary>
        private string helpCommandString;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedVariables"/> class.
        /// </summary>
        public FixedVariables()
        {
            // todo ok  could add specification for the individual file headers
            this.ArchiveHeaderOriginalLength = 30;
            this.ArchiveHeaderLength = this.ArchiveHeaderOriginalLength * 2;
            this.ArchiveHeaderDateTimeStartByteIndex = 0;
            this.ArchiveHeaderCompressionTypeStartByteIndex = 8;
            this.ArchiveHeaderNumberOfFilesStartByteIndex = 18;
            this.ArchiveHeaderSumOfFileSizeStartByteIndex = 22;
            this.ArchiveHeaderLengthOfCompressionCalling = this.ArchiveHeaderSumOfFileSizeStartByteIndex - this.ArchiveHeaderCompressionTypeStartByteIndex;

            // add to the compressioncalling siwtch case down below if this is ever expenet, its the only dependency
            this.CompressionCallingTypeNoCompression = "None";
            this.CompressionCallingTypeRLECompression = "RLE";
            this.ArchiveHeaderCompressionCallingFillerByte = 0;

            // if a path or the name of a file is longer than 5000 utf-8 bytes, that just means something was corrupted. Paths in windows can only be 255 long. Same with Filenames. Even in Linux filepaths can only be 4096 charachters long
            this.AbsoluteLimitBytesForFileNameAndPath = 5000;

            this.HelpCommandString =
                                   "\r\n\r\n\r\n" +
                                   "Supported Commands: \r\n" +

                                   " --create or -c: Create an archive. Required Parameters: -s -d. Source must be a directory. Destination can be either the filename or its path.\r\n" +
                                   " --extract or -x: Extract files from an archive. Required Parameters: -s -d. Source must be an archive file. Destination must be a directory.\r\n" +
                                   " --append or -a: Append to an existing archive. Required Parameters: -s -d. Source must be a directory. Destination must be an archive file.\r\n" +
                                   " --info or -i: Display metadata for the archive. Required Parameters: -s. Source must be an archive file.\r\n" +
                                   " --list or -l: List all files inside an archive. Required Parameters: -s. Source must be an archive file.\r\n" +
                                   " --help or -h: Show help tooltips. No required or optional parameters.\r\n\r\n" +

                                   "Supported Parameters: \r\n" +

                                   "--source or -s: Provide a source for the operation. Specification depends on the command.\r\n" +
                                   "--destination or -d: Specify a destination for the operation. Specification depends on the command.\r\n" +
                                   "--rleCompress or -rle: Use with 'create' to apply RLE Compression to the files.\r\n" +
                                   "--wait or -w: Define wait time (in seconds) between command retries. Specification: number between 1 and 10. Default is 1 if unspecified.\r\n" +
                                   "--retry or -r: Specify the number of command retries. Specification: number between 1 and 10. Default is 1 if unspecified.\r\n\r\n" +

                                   "Usage: \r\n" +

                                   "Commands can be executed sequentially. Each command requires specific parameters with valid information. \r\n" +
                                   "Example of a valid command: --create --source [path to the directory you want to compress] --destination [either the filename or its path].\r\n\r\n" +

                                   "Chaining Commands:\r\n" +

                                   "Example: -c -s [directory] -d [filename] -rle -a -s [second directory] -d [directory/filename for archive] -x -s [directory/filename for archive] -d [third directory]\r\n" +
                                   "This would create a new RLE-compressed archive in the directory, append files from the second directory, and then extract the contents into the third directory.";
        }

        /// <summary>
        /// Gets the archive header length inside the program.
        /// </summary>
        /// <value> The archive header length inside the program .</value>
        public int ArchiveHeaderLength
        {
            get
            {
                return this.archiveHeaderLength;
            }

            private set
            {
                this.archiveHeaderLength = value;
            }
        }

        /// <summary>
        /// Gets the archive header length without any parity bytes inside the program.
        /// </summary>
        /// <value> The archive header length without any parity bytes inside the program .</value>
        public int ArchiveHeaderOriginalLength
        {
            get
            {
                return this.archiveHeaderOriginalLength;
            }

            private set
            {
                this.archiveHeaderOriginalLength = value;
            }
        }

        /// <summary>
        /// Gets the index of the first byte of the date time inside the archive header.
        /// </summary>
        /// <value> The index of the first byte of the date time inside the archive header. </value>
        public int ArchiveHeaderDateTimeStartByteIndex
        {
            get
            {
                return this.archiveHeaderDateTimeStartByteIndex;
            }

            private set
            {
                this.archiveHeaderDateTimeStartByteIndex = value;
            }
        }

        /// <summary>
        /// Gets the index of the first byte of the compression type inside the archive header.
        /// </summary>
        /// <value> The index of the first byte of the compression type inside the archive header. </value>
        public int ArchiveHeaderCompressionTypeStartByteIndex
        {
            get
            {
                return this.archiveHeaderCompressionTypeStartByteIndex;
            }

            private set
            {
                this.archiveHeaderCompressionTypeStartByteIndex = value;
            }
        }

        /// <summary>
        /// Gets the index of the first byte of the number of files attribute inside the archive header.
        /// </summary>
        /// <value> The index of the first byte of the number of files attribute inside the archive header. </value>
        public int ArchiveHeaderNumberOfFilesStartByteIndex
        {
            get
            {
                return this.archiveHeaderNumberOfFilesStartByteIndex;
            }

            private set
            {
                this.archiveHeaderNumberOfFilesStartByteIndex = value;
            }
        }

        /// <summary>
        /// Gets the index of the first byte of the sum of files size attribute inside the archive header.
        /// </summary>
        /// <value> The index of the first byte of the sum of files size attribute inside the archive header. </value>
        public int ArchiveHeaderSumOfFileSizeStartByteIndex
        {
            get
            {
                return this.archiveHeaderSumOfFileSizeStartByteIndex;
            }

            private set
            {
                this.archiveHeaderSumOfFileSizeStartByteIndex = value;
            }
        }

        /// <summary>
        /// Gets the number of bytes used for the  length of the compression calling.
        /// </summary>
        /// <value> The number of bytes used for the  length of the compression calling. </value>
        public int ArchiveHeaderLengthOfCompressionCalling
        {
            get
            {
                return this.archiveHeaderLengthOfCompressionCalling;
            }

            private set
            {
                this.archiveHeaderLengthOfCompressionCalling = value;
            }
        }

        /// <summary>
        /// Gets the string of the calling for no compression.
        /// </summary>
        /// <value> The string of the calling for no compression. </value>
        public string CompressionCallingTypeNoCompression
        {
            get
            {
                return this.compressionCallingTypeNoCompression;
            }

            private set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.CompressionCallingTypeNoCompression)} cannot be null!");
                }

                this.compressionCallingTypeNoCompression = value;
            }
        }

        /// <summary>
        /// Gets the string of the calling for RLE compression.
        /// </summary>
        /// <value> The string of the calling for RLE compression. </value>
        public string CompressionCallingTypeRLECompression
        {
            get
            {
                return this.compressionCallingTypeRLECompression;
            }

            private set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.CompressionCallingTypeRLECompression)} cannot be null!");
                }

                this.compressionCallingTypeRLECompression = value;
            }
        }

        /// <summary>
        /// Gets the byte that is used as a filler byte inside the archive header if the compression calling is smaller that the compression calling byte array length.
        /// </summary>
        /// <value> The byte that is used as a filler byte inside the archive header if the compression calling is smaller that the compression calling byte array length. </value>
        public byte ArchiveHeaderCompressionCallingFillerByte
        {
            get
            {
                return this.archiveHeaderCompressionCallingFillerByte;
            }

            private set
            {
                this.archiveHeaderCompressionCallingFillerByte = value;
            }
        }

        /// <summary>
        /// Gets the the absolute limit a file name or path can have inside this application.
        /// </summary>
        /// <value> The the absolute limit a file name or path can have inside this application. </value>
        public int AbsoluteLimitBytesForFileNameAndPath
        {
            get
            {
                return this.absoluteLimitBytesForFileNameAndPath;
            }

            private set
            {
                this.absoluteLimitBytesForFileNameAndPath = value;
            }
        }

        /// <summary>
        /// Gets the string that is displayed on the console, when the help command is executed.
        /// </summary>
        /// <value> The string that is displayed on the console, when the help command is executed. </value>
        public string HelpCommandString
        {
            get
            {
                return this.helpCommandString;
            }

            private set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.HelpCommandString)} cannot be null!");
                }

                this.helpCommandString = value;
            }
        }
        
        /// <summary>
        /// This method returns the corresponding compression algorithm from a string that represents the compression calling or acronym.
        /// </summary>
        /// <param name="calling"> The acronym or calling of the compression.</param>
        /// <returns> The corresponding <see cref="ICompressionAlgorithm"/> associated with the calling. If no such algorithm exists it returns null.</returns>
        public ICompressionAlgorithm GetCompressionAlgorithmFromCalling(string calling)
        {
            CurrentlyWorkingCommandsAndCompressionsForArchiver currentlyWorkingCommandsAndCompressionsForArchiver = new CurrentlyWorkingCommandsAndCompressionsForArchiver();
            List<ICompressionAlgorithm> currentCompressionAlgorithms = currentlyWorkingCommandsAndCompressionsForArchiver.ReturnCurrentlyWorkingCompressionAlgorithms();

            foreach (ICompressionAlgorithm algorithm in currentCompressionAlgorithms)
            {
                if (calling.Equals(algorithm.ReturnCompressionTypeCalling()))
                {
                    // todo ok in the future it would be wise to not give the object by refrence, but make a deep copy
                    return algorithm;
                }
            }

            // the one class that uses this, thrown an exception if it happens, still not a good solution, todo ok turn into a tryparse method
            return null;
        }
    }
}