﻿//-----------------------------------------------------------------------
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
    /// This class is the append command. When executed it appends files inside a given diretory to an archive file, and modifies the archives main header to reflect changes.
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

        public ArchiveAppendCommand(string sourcePathToDirectory, string archiveFilePath)
        {
            this.ArchiveFilePath = archiveFilePath;
            this.SourcePathToDirectory = sourcePathToDirectory;
        }

        /// <summary>
        /// Gets or sets the source path to the directory from which to append files from.
        /// </summary>
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

                List<FileMetaInformation> fileMetaInfoList = directorySourceProcessor.CreateFileMetaInfoListForDirectory(compressionAlgorithm, fileNamesToSkip);

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

        private ArchiveHeader ModifyArchiveHeaderForAdditionalFiles(ArchiveHeader archiveHeader, List<FileMetaInformation> fileMetaInformationList)
        {
            DateTime modifiedArchiveHeaderDateTime = archiveHeader.TimeOfCreation;
            long modifiedArchiveSumOfFileBytes = archiveHeader.SizeOfFilesCombined + this.GetSumOfSizeForAllFilesCompressed(fileMetaInformationList);
            int modifiedArchiveNumberOfFiles = archiveHeader.NumberOfFilesInArchive + fileMetaInformationList.Count;

            // kinda weird hack/workaround but the first time we got this homework we didnt already know how to serialize, so im prociding as if that still is the case.
            string compressionAlgorithmUsedCalling = archiveHeader.CompressionTypeCalling;

            return new ArchiveHeader(modifiedArchiveHeaderDateTime, modifiedArchiveNumberOfFiles, compressionAlgorithmUsedCalling, modifiedArchiveSumOfFileBytes);
        }
    }
}