﻿namespace FileCompressor
{
    //TODO THIS NEEDS TO CHANGE TO SOMETHING ELse, ITS IMPORTANT THAT I have all of the information in somekind of class but its kinda retarded
    internal class FixedVariables
    {
        public int ArchiveHeaderLength { get; private set; }
        public int ArchiveHeaderDateTimeStartByteIndex { get; private set; }
        public int ArchiveHeaderCompressionTypeStartByteIndex { get; private set; }
        public int ArchiveHeaderNumberOfFilesStartByteIndex { get; private set; }
        public int ArchiveHeaderSumOfFileSizeStartByteIndex { get; private set; }
        public int ArchiveHeaderLengthOfCompressionCalling { get; private set; }
        public string CompressionCallingTypeNoCompression { get; private set; }
        public string CompressionCallingTypeRLECompression { get; private set; }
        public byte ArchiveHeaderCompressionCallingFillerByte { get; private set; }
        public int AbsoluteLimitBytesForFileNameAndPath { get; private set; }
        public string HelpCommandString { get; private set; }

        public FixedVariables()
        {
            this.ArchiveHeaderLength = 30;
            this.ArchiveHeaderDateTimeStartByteIndex = 0;
            this.ArchiveHeaderCompressionTypeStartByteIndex = 8;
            this.ArchiveHeaderNumberOfFilesStartByteIndex = 18;
            this.ArchiveHeaderSumOfFileSizeStartByteIndex = 22;
            this.ArchiveHeaderLengthOfCompressionCalling = this.ArchiveHeaderSumOfFileSizeStartByteIndex - this.ArchiveHeaderCompressionTypeStartByteIndex;

            //add to the compressioncalling siwtch case down below if this is ever expenet, its the only dependency

            this.CompressionCallingTypeNoCompression = "None";
            this.CompressionCallingTypeRLECompression = "RLE";
            this.ArchiveHeaderCompressionCallingFillerByte = 0;
            //bruh if a path or the name of a file is longer than 5000 utf-8 bytes that shit is corrupted. Paths in windows can only be 255 long. Same with Filenames. Even in Linux filepaths can only be 4096 charachters long
            this.AbsoluteLimitBytesForFileNameAndPath = 5000;

            this.HelpCommandString =
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

            //TODO ADD   // fileSIzeName(int)+ filename + filepathsize(INT) + FileRelativePath + filesizeoriginal (long) + filesizecompressed (long)
            //////long sumOfNumberOfBytesForFileHeader = 4 + this.FileNameSize + 4 + this.FileRelativePathSize + 8 + 8;
            //////byte[] fileHeaderAsBytes = new byte[sumOfNumberOfBytesForFileHeader];

            //////byte[] fileNameSizeAsBytes = BitConverter.GetBytes(this.FileNameSize);
            //////byte[] fileNameAsBytes = Encoding.UTF8.GetBytes(this.FileName);
            //////byte[] filePathSizeAsBytes = BitConverter.GetBytes(this.FileRelativePathSize);
            //////byte[] filePathAsBytes = Encoding.UTF8.GetBytes(this.FileRelativePath);
            //////byte[] fileOriginialSizeAsBytes = BitConverter.GetBytes(this.FileSizeOriginal);
            //////byte[] fileCompressionSizeAsBytes = BitConverter.GetBytes(this.FileSizeCompressed);

            //////fileNameSizeAsBytes.CopyTo(fileHeaderAsBytes, 0);
            //////fileNameAsBytes.CopyTo(fileHeaderAsBytes, 4);
            //////filePathSizeAsBytes.CopyTo(fileHeaderAsBytes, 4 + this.FileNameSize);
            //////filePathAsBytes.CopyTo(fileHeaderAsBytes, 4 + this.FileNameSize + 4);
            //////fileOriginialSizeAsBytes.CopyTo(fileHeaderAsBytes, 4 + this.FileNameSize + 4 + this.FileRelativePathSize);
            //////fileCompressionSizeAsBytes.CopyTo(fileHeaderAsBytes, 4 + this.FileNameSize + 4 + this.FileRelativePathSize + 8);

            //////return fileHeaderAsBytes;
        }

        public ICompressionAlgorithm GetCompressionAlgorithmFromCalling(string calling)
        {
            //switch ceas can only take contant values
            //TODO EXCEPTIONS
            if (calling.Equals(this.CompressionCallingTypeRLECompression))
            {
                return new RLECompressionAlgorithm();
            }
            else if (calling.Equals(this.CompressionCallingTypeNoCompression))
            {
                return new NoCompressionAlgorithm();
            }
            else
            {
                return null;
            }
        }
    }
}