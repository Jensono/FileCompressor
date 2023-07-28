//-----------------------------------------------------------------------
// <copyright file="ArchiveAppendCommand.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is the append command. When executed it appends files inside a given diretory to an archive file, and modifies the archives main header to reflect changes.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;

    // BIG ASS TODO this command needs to first create a copy of the file that needs to be appended and then delete the original file after the append happend. Otherwise the weirdest shit could happen while appending.
    // the destination for the create command is only the foldername and file ending eg.: archive.dat, archive.jth

    // TODO TODO check the parameter properties source and destination can not be null compression can be
    // OR - IF THE REQUIRED PARAMETERS ARE THERE ARE ALREADY CHECKED IN THE PROCESS OUTSIDE; BUT THEY DONT WANT THAT NORMALLY.

    /// <summary>
    /// This class is the append command. When executed it appends files inside a given directory to an archive file, and modifies the archives main header to reflect changes.
    /// </summary>
    public class ArchiveAppendCommand : IArchiveCommand
    {
        /// <summary>
        /// The field for the source path to the directory from which to append files from.
        /// </summary>
        private string sourcePathToDirectory;

        /// <summary>
        /// The field for the path to the archive file to which to append the new files.
        /// </summary>
        private string archiveFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveAppendCommand"/> class. 
        /// </summary>
        /// <param name="sourcePathToDirectory"> The path to the source directory that should be appended as a string.</param>
        /// <param name="archiveFilePath"> The path to the archive that should be appended as a string.</param>
        public ArchiveAppendCommand(string sourcePathToDirectory, string archiveFilePath)
        {
            this.ArchiveFilePath = archiveFilePath;
            this.SourcePathToDirectory = sourcePathToDirectory;
        }

        /// <summary>
        /// Gets or sets the source path to the directory from which to append files from.
        /// </summary>
        /// <value> The source path to the directory from which to append files from. </value>
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
        /// Gets or sets the path to the archive file to which to append the new files.
        /// </summary>
        /// <value> The path to the archive file to which to append the new files. </value>
        public string ArchiveFilePath
        {
            get
            {
                return this.archiveFilePath;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ArchiveFilePath)} cannot be null!");
                }

                this.archiveFilePath = value;
            }
        }        

        /// <summary>
        /// This method calculates the sum of all the files if they were to be compressed with the given compression algorithm.
        /// </summary>
        /// <param name="fileList"> The files for which to calculates the sum of all bytes , if compressed.</param>
        /// <returns> The sum of the compressed sizes for the files.</returns>
        /// <exception cref="ArgumentNullException"> Is thrown if the given file list is null.</exception>
        public long GetSumOfSizeForAllFilesCompressed(List<FileMetaInformation> fileList)
        {
            if (fileList is null)
            {
                throw new ArgumentNullException($"{nameof(fileList)} can not be null!");
            }

            long sum = 0;
            foreach (FileMetaInformation fileMetaInfo in fileList)
            {
                sum += fileMetaInfo.Length;
            }

            return sum;
        }

        /// <summary>
        /// This method executes the append process with the parameters given when creating the class.
        /// </summary>
        /// <returns> A boolean value indicating whether or not the execution succeeded.</returns>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown when there way a Archive error thrown during the execution process.</exception>
        public bool Execute()
        {
            // TODO WHEN OVERWRITTING a file first needs to be safed under some kind of temporary name, if while the file should be overwritten there is an error both are lost!!!!!
            try
            {
                ArchiveFileReader archiveFileReader = new ArchiveFileReader(this.ArchiveFilePath);
                ICompressionAlgorithm compressionAlgorithm = archiveFileReader.CompressionAlogrithmenUsed;

                DirectorySourceProcessor directorySourceProcessor = new DirectorySourceProcessor(this.SourcePathToDirectory);

                // just needed for the creation command, when creating the list it skips over all files with names inside the array
                // for now just add the archivename itself, so it doesnt try to copy itself when using this command
                string[] fileNamesToSkip = new string[] { this.ArchiveFilePath };

                List<FileMetaInformation> fileMetaInfoList = directorySourceProcessor.CreateFileMetaInfoListForDirectory(fileNamesToSkip);

                ArchiveHeader currentArchiveHeader = new ArchiveHeader(fileMetaInfoList.Count, compressionAlgorithm.ReturnCompressionTypeCalling(), this.GetSumOfSizeForAllFilesCompressed(fileMetaInfoList));
                ArchiveFileWriter archiveFileWriter = new ArchiveFileWriter(this.SourcePathToDirectory, this.ArchiveFilePath, compressionAlgorithm);

                ArchiveHeader newArchiveHeader = this.ModifyArchiveHeaderForAdditionalFiles(archiveFileReader.ReturnArchiveHeader(), fileMetaInfoList);

                // read the archiveheader and modify it to fit more files.

                // disk spaced is checked inside this method before writing
                archiveFileWriter.AppendToArchive(this.ArchiveFilePath, fileMetaInfoList, newArchiveHeader);
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }

            return true;
        }

        /// <summary>
        /// This method modifies the old Archive header inside the archive file to reflect the append of new files.
        /// </summary>
        /// <param name="oldArchiveHeader"> The old archive header before appending new files. </param>
        /// <param name="fileMetaInformationList"> The list of files which will be appended during the append command.</param>
        /// <returns> A modified Archive Header with adjusted information for the append. New sum number of files , sum number of uncompressed bytes and so forth.</returns>
        private ArchiveHeader ModifyArchiveHeaderForAdditionalFiles(ArchiveHeader oldArchiveHeader, List<FileMetaInformation> fileMetaInformationList)
        {
            DateTime modifiedArchiveHeaderDateTime = oldArchiveHeader.TimeOfCreation;
            long modifiedArchiveSumOfFileBytes = oldArchiveHeader.SizeOfFilesCombined + this.GetSumOfSizeForAllFilesCompressed(fileMetaInformationList);
            int modifiedArchiveNumberOfFiles = oldArchiveHeader.NumberOfFilesInArchive + fileMetaInformationList.Count;

            // kinda weird hack/workaround but the first time we got this homework we didnt already know how to serialize, so im prociding as if that still is the case.
            string compressionAlgorithmUsedCalling = oldArchiveHeader.CompressionTypeCalling;

            return new ArchiveHeader(modifiedArchiveHeaderDateTime, modifiedArchiveNumberOfFiles, compressionAlgorithmUsedCalling, modifiedArchiveSumOfFileBytes);
        }
    }
}