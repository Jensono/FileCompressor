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

    public class FileMetaInformation
    {
        
        private string fullName;
        private string name;
        private long length;
        private string relativePathForArchive;

        public FileMetaInformation(FileInfo fileInfo, string relativePathOfFileInsideArchive)
        {
            this.FullName = fileInfo.FullName;
            this.Name = fileInfo.Name;
            this.Length = fileInfo.Length;
            this.RelativePathForArchive = relativePathOfFileInsideArchive;
        }

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