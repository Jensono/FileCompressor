//-----------------------------------------------------------------------
// <copyright file="IndividualFileHeaderInformation.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class contains describtion on what meta information will be saved inside the archive for each indiviual file contained.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Text;
    public class IndividualFileHeaderInformation
    {
        // sizes of the path and name as 8 bytes respectivly

        // could also just use unsigned int to get more bytes
        private string name;
        private string relativePath;
        private long sizeOriginal;
        private long sizeCompressed;
        private int nameSize;
        private int relativePathSize;

        public IndividualFileHeaderInformation(string givenFileName, string givenFileRelativePath, long givenFileSizeOriginal, long givenFileSizeCompressed)
        {
            this.Name = givenFileName;
            this.RelativePath = givenFileRelativePath;
            this.SizeOriginal = givenFileSizeOriginal;

            // the file size with compression is first set to the original size, where ever this object is created. there is a loop outside that that after writing the bytes to the .dat file, returns to the individualfile header and
            // rewrites the correct size, given that we already use long there is no way a size bigger than that is created.
            this.SizeCompressed = givenFileSizeCompressed;
            this.NameSize = Encoding.UTF8.GetByteCount(givenFileName);
            this.RelativePathSize = Encoding.UTF8.GetByteCount(givenFileRelativePath);
        }

        public string Name
        {
            get 
            {
                return this.name;
            }

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

        public string RelativePath
        {
            get 
            {
                return this.relativePath;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException($"{nameof(this.RelativePath)} cannot be null!");
                }

                this.relativePath = value;
            }
        }
       

        public long SizeOriginal
        {
            get 
            {
                return this.sizeOriginal; 
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.SizeOriginal)} cannot be negative!");
                }

                this.sizeOriginal = value;
            }
        }
        

        public long SizeCompressed
        {
            get
            {
                return this.sizeCompressed; 
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.SizeCompressed)} cannot be negative!");
                }

                this.sizeCompressed = value;
            }
        }

        public int NameSize
        {
            get 
            {
                return this.nameSize;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.NameSize)} cannot be negative!");
                }

                this.nameSize = value;
            }
        }

        public int RelativePathSize
        {
            get 
            { 
                return this.relativePathSize;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.RelativePathSize)} cannot be negative!");
                }

                this.relativePathSize = value;
            }
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

            fileNameSizeAsBytes.CopyTo(fileHeaderAsBytes, 0);
            fileNameAsBytes.CopyTo(fileHeaderAsBytes, 4);
            filePathSizeAsBytes.CopyTo(fileHeaderAsBytes, 4 + this.NameSize);
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