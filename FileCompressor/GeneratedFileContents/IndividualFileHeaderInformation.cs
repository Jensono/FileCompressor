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

    /// <summary>
    ///  This class contains description on what meta information will be saved inside the archive for each individual file contained.
    /// </summary>
    public class IndividualFileHeaderInformation
    {
        /// <summary>
        /// The field for the name of the file for which the file header is created.
        /// </summary>
        private string name;

        /// <summary>
        /// The field for the relative path of the file.
        /// </summary>
        private string relativePath;

        /// <summary>
        /// The field for the uncompressed size of the file.
        /// </summary>
        private long sizeOriginal;

        /// <summary>
        /// The field for the size of the file after compression in bytes.
        /// </summary>
        private long sizeCompressed;

        /// <summary>
        /// The field for the length of the name of the file in bytes.
        /// </summary>
        private int nameSize;

        /// <summary>
        /// The field for the length of the relative path of the file in bytes.
        /// </summary>
        private int relativePathSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndividualFileHeaderInformation"/> class.
        /// </summary>
        /// <param name="givenFileName"> The file name that the file header will contain for the file. </param>
        /// <param name="givenFileRelativePath"> The relative path of the file.</param>
        /// <param name="givenFileSizeOriginal"> The file size of the original file.</param>
        /// <param name="givenFileSizeCompressed"> The file size of the file compressed inside the archive.</param>
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

        /// <summary>
        /// Gets or sets the name of the file for which the file header is created.
        /// </summary>
        /// <value> The name of the file for which the file header is created. </value>
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

        /// <summary>
        /// Gets or sets the relative path of the file.
        /// </summary>
        /// <value> The relative path of the file. </value>
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

        /// <summary>
        /// Gets or sets the uncompressed size of the file.
        /// </summary>
        /// <value> The uncompressed size of the file. </value>
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

        /// <summary>
        /// Gets or sets the size of the file after compression in bytes.
        /// </summary>
        /// <value> The size of the file after compression in bytes. </value>
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

        /// <summary>
        /// Gets or sets the length of the name of the file in bytes.
        /// </summary>
        /// <value> The length of the name of the file in bytes. </value>
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

        /// <summary>
        /// Gets or sets the length of the relative path of the file in bytes.
        /// </summary>
        /// <value> The length of the relative path of the file in bytes. </value>
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