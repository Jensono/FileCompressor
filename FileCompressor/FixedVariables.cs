using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    //TODO THIS NEEDS TO CHANGE TO SOMETHING ELse, ITS IMPORTANT THAT I have all of the information in somekind of class but its kinda retarded
    class FixedVariables
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
