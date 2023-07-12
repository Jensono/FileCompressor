using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    class IndividualFileHeaderInformation
    {


        //sizes of the path and name as 8 bytes respectivly

        //could also just use unsigned int to get more bytes

        private string fileName;
        public string FileName
        {
            get { return this.fileName; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException($"{nameof(this.FileName)} cannot be null!");
                }
                this.fileName = value;
            }
        }

        private string fileRelative;
        public string FileRelativePath
        {
            get { return this.fileRelative; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException($"{nameof(this.FileRelativePath)} cannot be null!");
                }
                this.fileRelative = value;
            }
        }

        private long fileSizeOriginal;
        public long FileSizeOriginal
        {
            get { return this.fileSizeOriginal; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.FileSizeOriginal)} cannot be negative!");
                }
                this.fileSizeOriginal = value;
            }
        }

        private long fileSizeCompressed;
        public long FileSizeCompressed
        {
            get { return this.fileSizeCompressed; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.FileSizeCompressed)} cannot be negative!");
                }
                this.fileSizeCompressed = value;
            }
        }

        private int fileNameSize;
        public int FileNameSize
        {
            get { return this.fileNameSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.FileNameSize)} cannot be negative!");
                }
                this.fileNameSize = value;
            }
        }

        private int fileRelativePathSize;
        public int FileRelativePathSize
        {
            get { return this.fileRelativePathSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.FileRelativePathSize)} cannot be negative!");
                }
                this.fileRelativePathSize = value;
            }
        }

        //TODO filepath and filename validity is not checked here
        // file path and name have to be converted to utf-8 before beeing processed
        //all Sizes are in Bytes
        public IndividualFileHeaderInformation(string givenFileName,string givenFileRelativePath,long givenFileSizeOriginal,long givenFileSizeCompressed)
        {

            this.FileName = givenFileName;
            this.FileRelativePath = givenFileRelativePath;
            this.FileSizeOriginal = givenFileSizeOriginal;
            //the file size with compression is first set to the original size, where ever this object is created. there is a looop outside that that after writing the bytes to the .dat file, returns to the individualfile header and 
            // rewrites the correct size, given that we already use long there is no way a size bigger than that is created.
            this.FileSizeCompressed = givenFileSizeOriginal;
            this.FileNameSize = Encoding.UTF8.GetByteCount(givenFileName);
            this.FileRelativePathSize = Encoding.UTF8.GetByteCount(givenFileRelativePath);



        }

        public byte[] GetFileHeaderAsByteArray()      
        {
            // fileSIzeName(int)+ filename + filepathsize(INT) + FileRelativePath + filesizeoriginal (long) + filesizecompressed (long)
            long sumOfNumberOfBytesForFileHeader = 4 + this.FileNameSize + 4 + this.FileRelativePathSize + 8 + 8;
            byte[] fileHeaderAsBytes = new byte[sumOfNumberOfBytesForFileHeader];

            byte[] fileNameSizeAsBytes = BitConverter.GetBytes(this.FileNameSize);
            byte[] fileNameAsBytes = Encoding.UTF8.GetBytes(this.FileName);
            byte[] filePathSizeAsBytes = BitConverter.GetBytes(this.FileRelativePathSize);
            byte[] filePathAsBytes = Encoding.UTF8.GetBytes(this.FileRelativePath);
            byte[] fileOriginialSizeAsBytes = BitConverter.GetBytes(this.FileSizeOriginal);
            byte[] fileCompressionSizeAsBytes = BitConverter.GetBytes(this.FileSizeCompressed);

            fileNameSizeAsBytes.CopyTo(fileHeaderAsBytes,0);
            fileNameAsBytes.CopyTo(fileHeaderAsBytes, 4);
            filePathSizeAsBytes.CopyTo(fileHeaderAsBytes, 4+this.FileNameSize);
            filePathAsBytes.CopyTo(fileHeaderAsBytes, 4 + this.FileNameSize + 4);
            fileOriginialSizeAsBytes.CopyTo(fileHeaderAsBytes, 4 + this.FileNameSize + 4 + this.FileRelativePathSize);
            fileCompressionSizeAsBytes.CopyTo(fileHeaderAsBytes, 4 + this.FileNameSize + 4 + this.FileRelativePathSize + 8);

            return fileHeaderAsBytes;






        }


    }
}
