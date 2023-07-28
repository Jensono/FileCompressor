//-----------------------------------------------------------------------
// <copyright file="ExtractArchiveCommand.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is the command for exctracting a archive file. When executed it extracts all files contained in a given source archive file into a destination folder.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;

    /// <summary>
    /// This class is the command for extracting a archive file. When executed it extracts all files contained in a given source archive file into a destination folder.
    /// </summary>
    public class ExtractArchiveCommand : IArchiveCommand
    {
        /// <summary>
        /// The field for the destination path into which to extract the archive into.
        /// </summary>
        private string destinationPathToDirectory;

        /// <summary>
        /// The field for the path to the source (the archive) from which to extract files.
        /// </summary>
        private string archiveSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractArchiveCommand"/> class. 
        /// </summary>
        /// <param name="archiveSource"> The path to the archive that should be extracted as a string.</param>
        /// <param name="destination"> The destination folder into which the files should be decompressed.</param>
        public ExtractArchiveCommand(string archiveSource, string destination)
        {
            this.DestinationPathToDirectory = destination;
            this.ArchiveSource = archiveSource;
        }

        /// <summary>
        /// Gets or sets the destination path into which to extract the archive into.
        /// </summary>
        /// <value> The destination path into which to extract the archive into.</value>
        public string DestinationPathToDirectory
        {
            get
            {
                return this.destinationPathToDirectory;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.DestinationPathToDirectory)} cannot be null!");
                }

                this.destinationPathToDirectory = value;
            }
        }

        /// <summary>
        /// Gets or sets the path to the source (the archive) from which to extract files.
        /// </summary>
        /// <value> The path to the source (the archive) from which to extract files.</value>
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
        /// This method executes the extract command. It tries to read all files from the archive and extract them one by one into the specified directory.
        /// </summary>
        /// <returns> A boolean value indicating whether or not the execution was successful.</returns>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown if during the execution a archive error is caught. </exception>
        public bool Execute()
        {
            try
            {
                ArchiveFileReader archiveReader = new ArchiveFileReader(this.ArchiveSource);

                archiveReader.ExtractArchiveFiles(this.DestinationPathToDirectory);
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;

                ////return false;
            }

            return true;
        }
    }
}