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

        /// <summary>
        /// Initializes a new instance of the <see cref="ListArchiveContentsCommand"/> class. 
        /// </summary>
        /// <param name="source"> The path to the Archive file from which to list information about.</param>
        public ListArchiveContentsCommand(string source)
        {
            this.ArchiveSource = source;
        }

        /// <summary>
        /// Gets or sets the path to the archive from which to extract information of the files inside.
        /// </summary>
        /// <value> The path to the archive from which to extract information of the files inside.</value>
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

        /// <summary>
        /// This method executes the list content command. It reads all individual file header in the archive, makes a list of the names and prints them to the console
        /// </summary>
        /// <returns> A boolean value indicating whether or not the execution was succesful. </returns>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown if , during execution a archive error was caught.</exception>
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

        /// <summary>
        /// This method reads all the files inside a archive file and and lists the found names as string to the console.
        /// </summary>
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