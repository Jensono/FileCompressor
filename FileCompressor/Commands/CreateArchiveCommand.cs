//-----------------------------------------------------------------------
// <copyright file="CreateArchiveCommand.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is the command to create an archive. When executed it creates a "compressed" file for a given directory. The resulting file contains all files given in the directory, meta information about the files contained,
// and meta information about the archive itself.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// This class is the command to create an archive. When executed it creates a "compressed" file for a given directory. The resulting file contains all files given in the directory, meta information about the files contained,
    /// and meta information about the archive itself.
    /// </summary>
    public class CreateArchiveCommand : IArchiveCommand
    {
        // source and destination, source is a path to a directory that should be compressed. Destination is just a file name that needs a ending. but maybe one should just be able to sepcify a name only
        // TODO BOTH NEED TO BE CHECKED BEFORE STARTING THE CREATION PROCESS

        /// <summary>
        /// The field for the path to the directory from which to compress files into an archive.
        /// </summary>
        private string sourcePathToDirectory;

        /// <summary>
        /// The field for the destination name of the resulting archive file.
        /// </summary>
        private string destinationNameForFile;

        /// <summary>
        /// The field for the <see cref="ICompressionAlgorithm"/> compression algorithm used for the compression of files.
        /// </summary>
        private ICompressionAlgorithm usedCompression;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateArchiveCommand"/> class. 
        /// </summary>
        /// <param name="sourcePathToDirectory"> The source path to the directory from which to compress files from.</param>
        /// <param name="destinationNameForTheFile"> The name for the resulting archive file as a string.</param>
        /// <param name="compressionAlgorithm"> The compress algorithm used for the archive.</param>
        public CreateArchiveCommand(string sourcePathToDirectory, string destinationNameForTheFile, ICompressionAlgorithm compressionAlgorithm)
        {
            this.UsedCompression = compressionAlgorithm;
            this.DestinationNameForFile = destinationNameForTheFile;
            this.SourcePathToDirectory = sourcePathToDirectory;
        }

        /// <summary>
        /// Gets or sets the path to the directory from which to compress files into an archive.
        /// </summary>
        /// <value> The path to the directory from which to compress files into an archive.</value>
        public string SourcePathToDirectory
        {
            get
            {
                return this.sourcePathToDirectory;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.SourcePathToDirectory)} cannot be null!");
                }

                this.sourcePathToDirectory = value;
            }
        }

        /// <summary>
        /// Gets or sets the destination name of the resulting archive file.
        /// </summary>
        /// <value> The destination name of the resulting archive file. </value>
        public string DestinationNameForFile
        {
            get
            {
                return this.destinationNameForFile;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.DestinationNameForFile)} cannot be null!");
                }

                this.destinationNameForFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ICompressionAlgorithm"/> compression algorithm used for the compression of files.
        /// </summary>
        /// <value> The <see cref="ICompressionAlgorithm"/> compression algorithm used for the compression of files.</value>
        public ICompressionAlgorithm UsedCompression
        {
            get
            {
                return this.usedCompression;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.UsedCompression)} cannot be null!");
                }

                this.usedCompression = value;
            }
        }

        /// <summary>
        /// This method executes the create command. It create a new archive file or overwrites the old fiel if it is not read only, and compressed all the given files into the archive. A archive consists of 3 
        /// major components. The Archive header, containg all the meta information about the contents of the archive etc., the individual file header for a file, detailing the name of the file, its size and path, and finally
        /// the file data itself. The archive header is only at the top and is the same for all compression types. After that follows indivual file header - file content- individual file header - file content ... and so forth.
        /// </summary>
        /// <returns> A boolean value indicating whether or not the execution was succesful.</returns>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown if a Archive error is thrown during execution.</exception>
        public bool Execute()
        {
            try
            {
                DirectorySourceProcessor directorySourceProcessor = new DirectorySourceProcessor(this.SourcePathToDirectory);
                string[] filesToSkip = new string[] { Path.Combine(this.SourcePathToDirectory, this.DestinationNameForFile) };
                List<FileMetaInformation> fileMetaInfoList = directorySourceProcessor.CreateFileMetaInfoListForDirectory(this.UsedCompression, filesToSkip);

                ArchiveHeader currentArchiveHeader = new ArchiveHeader(fileMetaInfoList.Count, this.UsedCompression.ReturnCompressionTypeCalling(), this.GetSumOfSizeForAllFilesCompressed(fileMetaInfoList));

                ArchiveFileWriter archiveFileWriter = new ArchiveFileWriter(this.SourcePathToDirectory, this.DestinationNameForFile, this.UsedCompression);

                // disk space is checked inside there
                archiveFileWriter.CreateArchive(currentArchiveHeader, fileMetaInfoList);
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }

            return true;

            // TODO WHEN OVERWRITTING a file first needs to be safed under some kind of temporary name, if while the file should be overwritten there is an error both are lost!!!!!
            // the system right now could by itself produce corrupted files, but it would require major user interfirance in the programm or the process itself.
        }

        /// <summary>
        /// This method returns the compressed file size sum of all files that will be contained in the archive. Mainly used for checking disk space.
        /// </summary>
        /// <param name="fileList"> The file list for which to calculate the sum for.</param>
        /// <returns> The calculated sum of bytes for all the files in their compressed state.</returns>
        public long GetSumOfSizeForAllFilesCompressed(List<FileMetaInformation> fileList)
        {
            long sum = 0;
            foreach (FileMetaInformation fileMetaInfo in fileList)
            {
                sum += fileMetaInfo.Length;
            }

            return sum;
        }
    }
}