//-----------------------------------------------------------------------
// <copyright file="ListArchiveContentsCommand.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is the list command for an archive. When executed it list all files contained inside of an archive file.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// This class is the list command for an archive. When executed it list all files contained inside of an archive file.
    /// </summary>
    public class ListArchiveContentsCommand : IArchiveCommand
    {
        /// <summary>
        /// The field for the path to the archive from which to extract information of the files inside.
        /// </summary>
        private string archiveSource;
        public ListArchiveContentsCommand(string source)
        {
            this.ArchiveSource = source;
        }

        /// <summary>
        /// Gets or sets the path to the archive from which to extract information of the files inside.
        /// </summary>
        public string ArchiveSource
        {
            get
            {
                return this.archiveSource;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ArchiveSource)} cannot be null!");
                }

                this.archiveSource = value;
            }
        }

        

        public bool Execute()
        {
            if (File.Exists(this.ArchiveSource))
            {
                try
                {
                    this.ReadArchiveFileAndListEntries();
                }
                catch (ArchiveErrorCodeException e)
                {
                    throw e;
                }
            }
            else
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. The file at {this.ArchiveSource} was not found. ");

                // return false;
            }

            return true;
        }

        private void ReadArchiveFileAndListEntries()
        {
            // method only utilizes the filerader
            List<string> entries = new List<string>();

            try
            {
                ArchiveFileReader archiveReader = new ArchiveFileReader(this.ArchiveSource);

                entries = archiveReader.ReadArchiveFileAndReturnEntries();
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }

            foreach (var item in entries)
            {
                Console.WriteLine(item);
            }
        }
    }
}