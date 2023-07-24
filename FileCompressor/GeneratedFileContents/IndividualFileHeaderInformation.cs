using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    public class IndividualFileHeaderInformation
    {


        //sizes of the path and name as 8 bytes respectivly

        //could also just use unsigned int to get more bytes

        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException($"{nameof(this.Name)} cannot be null!");
                }
                if (value.Equals(string.Empty))
                {
                    throw new ArgumentException("A filename can not be null!");
                }
                this.name = value;
            }
        }

        private string relativePath;
        public string RelativePath
        {
            get { return this.relativePath; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException($"{nameof(this.RelativePath)} cannot be null!");
                }
                this.relativePath = value;
            }
        }

        private long sizeOriginal;
        public long SizeOriginal
        {
            get { return this.sizeOriginal; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.SizeOriginal)} cannot be negative!");
                }
                this.sizeOriginal = value;
            }
        }

        private long sizeCompressed;
        public long SizeCompressed
        {
            get { return this.sizeCompressed; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.SizeCompressed)} cannot be negative!");
                }
                this.sizeCompressed = value;
            }
        }

        private int nameSize;
        public int NameSize
        {
            get { return this.nameSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.NameSize)} cannot be negative!");
                }
                this.nameSize = value;
            }
        }

        private int relativePathSize;
        public int RelativePathSize
        {
            get { return this.relativePathSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.RelativePathSize)} cannot be negative!");
                }
                this.relativePathSize = value;
            }
        }

        public IndividualFileHeaderInformation(string givenFileName,string givenFileRelativePath,long givenFileSizeOriginal,long givenFileSizeCompressed)
        {

            this.Name = givenFileName;
            this.RelativePath = givenFileRelativePath;
            this.SizeOriginal = givenFileSizeOriginal;
            //the file size with compression is first set to the original size, where ever this object is created. there is a looop outside that that after writing the bytes to the .dat file, returns to the individualfile header and 
            // rewrites the correct size, given that we already use long there is no way a size bigger than that is created.
            this.SizeCompressed = givenFileSizeCompressed;
            this.NameSize = Encoding.UTF8.GetByteCount(givenFileName);
            this.RelativePathSize = Encoding.UTF8.GetByteCount(givenFileRelativePath);
            //there is no way this should ever happen
            



        }

        public byte[] GetFileHeaderAsByteArray()      
        {
            // fileSIzeName(int)+ filename + filepathsize(INT) + FileRelativePath + filesizeoriginal (long) + filesizecompressed (long)
            long sumOfNumberOfBytesForFileHeader = 4 + this.NameSize + 4 + this.RelativePathSize + 8 + 8;
            byte[] fileHeaderAsBytes = new byte[sumOfNumberOfBytesForFileHeader];

            byte[] fileNameSizeAsBytes = BitConverter.GetBytes(this.NameSize);
            byte[] fileNameAsBytes = Encoding.UTF8.GetBytes(this.Name);
            byte[] filePathSizeAsBytes = BitConverter.GetBytes(this.RelativePathSize);
            byte[] filePathAsBytes = Encoding.UTF8.GetBytes(this.RelativePath);
            byte[] fileOriginialSizeAsBytes = BitConverter.GetBytes(this.SizeOriginal);
            byte[] fileCompressionSizeAsBytes = BitConverter.GetBytes(this.SizeCompressed);

            fileNameSizeAsBytes.CopyTo(fileHeaderAsBytes,0);
            fileNameAsBytes.CopyTo(fileHeaderAsBytes, 4);
            filePathSizeAsBytes.CopyTo(fileHeaderAsBytes, 4+this.NameSize);
            filePathAsBytes.CopyTo(fileHeaderAsBytes, 4 + this.NameSize + 4);
            fileOriginialSizeAsBytes.CopyTo(fileHeaderAsBytes, 4 + this.NameSize + 4 + this.RelativePathSize);
            fileCompressionSizeAsBytes.CopyTo(fileHeaderAsBytes, 4 + this.NameSize + 4 + this.RelativePathSize + 8);

            if (true)
            {

            }


            return fileHeaderAsBytes;






        }


    }
}
