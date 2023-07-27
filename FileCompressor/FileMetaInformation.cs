//-----------------------------------------------------------------------
// <copyright file="FileMetaInformation.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is used to compress needed file information for the archiver into one class.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.IO;

    /// <summary>
    /// This class is used to compress needed file information for the archive creator into one class.
    /// </summary>
    public class FileMetaInformation
    {        
        /// <summary>
        /// The field for the full name of the file. Meaning its full path , name and file ending.
        /// </summary>
        private string fullName;

        /// <summary>
        /// The field for the short name of the file. Only the name and file ending.
        /// </summary>
        private string name;

        /// <summary>
        /// The field for the the length of the size in bytes.
        /// </summary>
        private long length;

        /// <summary>
        /// The field for the relative path the file is located on, in relation to another path.
        /// </summary>
        private string relativePathForArchive;

        public FileMetaInformation(FileInfo fileInfo, string relativePathOfFileInsideArchive)
        {
            this.FullName = fileInfo.FullName;
            this.Name = fileInfo.Name;
            this.Length = fileInfo.Length;
            this.RelativePathForArchive = relativePathOfFileInsideArchive;
        }

        /// <summary>
        /// Gets the full name of the file. Meaning its full path , name and file ending.
        /// </summary>
        /// <value> The full name of the file. Meaning its full path , name and file ending. </value>
        public string FullName
        {
            get
            {
                return this.fullName;
            }

            private set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.FullName)} cannot be null!");
                }

                this.fullName = value;
            }
        }

        /// <summary>
        /// Gets the short name of the file. Only the name and file ending.
        /// </summary>
        /// <value> The short name of the file. Only the name and file ending.</value>
        public string Name
        {
            get
            {
                return this.name;
            }

            private set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.Name)} cannot be null!");
                }

                this.name = value;
            }
        }

        /// <summary>
        /// Gets the length of the size in bytes.
        /// </summary>
        /// <value> The length of the size in bytes.</value>
        public long Length
        {
            get
            {
                return this.length;
            }

            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.Length)} cannot be negative!");
                }

                this.length = value;
            }
        }

        /// <summary>
        /// Gets the relative path the file is located on, in relation to another path.
        /// </summary>
        /// <value> The relative path the file is located on, in relation to another path. </value>
        public string RelativePathForArchive
        {
            get
            {
                return this.relativePathForArchive;
            }

            private set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.RelativePathForArchive)} cannot be null!");
                }

                this.relativePathForArchive = value;
            }
        }
    }
}