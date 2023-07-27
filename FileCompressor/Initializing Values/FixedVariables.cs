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

    // TODO ok THIS NEEDS TO CHANGE TO SOMETHING ELse, ITS IMPORTANT THAT I have all of the information in somekind of class but its kinda retarded
    public class FixedVariables
    {
        private string givenSourceDirectory;
        private bool isSourceValid;
        private int archiveHeaderLength;
        private int archiveHeaderOriginalLength;
        private int archiveHeaderDateTimeStartByteIndex;
        private int archiveHeaderCompressionTypeStartByteIndex;
        private int archiveHeaderNumberOfFilesStartByteIndex;
        private int archiveHeaderSumOfFileSizeStartByteIndex;
        private int archiveHeaderLengthOfCompressionCalling;
        private string compressionCallingTypeNoCompression;
        private string compressionCallingTypeRLECompression;
        private byte archiveHeaderCompressionCallingFillerByte;
        private int absoluteLimitBytesForFileNameAndPath;
        private string helpCommandString;

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

            // bruh if a path or the name of a file is longer than 5000 utf-8 bytes that shit is corrupted. Paths in windows can only be 255 long. Same with Filenames. Even in Linux filepaths can only be 4096 charachters long
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