//-----------------------------------------------------------------------
// <copyright file="DirectorySourceProcessor.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class validates a given path, to make sure it is a valid directory and can extract further information form there.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    // this class is given a source and checks if the given string is a valid directory on the machine, if so it then can create a list of fileMetaInformation about the directory and give that information back

    /// <summary>
    /// This class validates a given path, to make sure it is a valid directory and can extract further information form there.
    /// </summary>
    public class DirectorySourceProcessor
    {
        /// <summary>
        /// The field for the source path to the directory that should be processed.
        /// </summary>
        private string givenSourceDirectory;

        /// <summary>
        /// This fields shows whether or not the given path source is a valid directory.
        /// </summary>
        private bool isSourceValid;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectorySourceProcessor"/> class. 
        /// </summary>
        /// <param name="sourceDirectory"> The source path to the directory that should be validated and or extracted information from.</param>
        public DirectorySourceProcessor(string sourceDirectory)
        {
            if (sourceDirectory is null || sourceDirectory == string.Empty)
            {
                throw new ArgumentException("Source directory must not be null or empty.");
            }

            this.GivenSourceDirectory = sourceDirectory;
            try
            {
                this.CheckForDirectoryValidity();
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }
           
            if (!this.IsSourceValid)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Given Path  {sourceDirectory} does not exist ");
            }
        }

        /// <summary>
        /// Gets the source path to the directory that should be processed.
        /// </summary>
        /// <value> The source path to the directory that should be processed. </value>
        public string GivenSourceDirectory
        {
            get
            {
                return this.givenSourceDirectory;
            }

            private set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.GivenSourceDirectory)} cannot be null!");
                }

                this.givenSourceDirectory = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the given path source is a valid directory.
        /// </summary>
        /// <value> The boolean value that shows whether or not the given path source is a valid directory. </value>
        public bool IsSourceValid
        {
            get
            {
                return this.isSourceValid;
            }

            private set
            {
                this.isSourceValid = value;
            }
        }

        

        /// <summary>
        /// This method creates a list of <see cref="FileMetaInformation"/> that is contained in the given directory.
        /// </summary> 
        /// <param name="filePathsToSkip"> A list of file paths that should be skipped , or for which not to generate a <see cref="FileMetaInformation"/> object. </param>
        /// <returns> A list of file meta information containing all the files (recursive search) in the given directory, excluding those which had 
        /// the same path as any of the excluded paths.
        /// </returns>
        /// <exception cref="ArchiveErrorCodeException"> 
        /// Is raised when: The given source is inside the class is not valid.
        ///                 A file in the given directory could not be processes for various reasons.
        ///                 A directory could not be processed for multiple reasons.         
        /// </exception>
        public List<FileMetaInformation> CreateFileMetaInfoListForDirectory(string[] filePathsToSkip)
        {
            try
            {
                this.CheckForDirectoryValidity();
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }
          
            List<FileMetaInformation> fileInfoList = new List<FileMetaInformation>();

            try
            {
                // with this option all files will be put into the string array - "*.*" just means that all types of files and all names are valid.
                string[] fileArray = Directory.GetFiles(this.GivenSourceDirectory, "*.*", SearchOption.AllDirectories);

                // removing all the entries in the fileArray that contains paths that are also in the string[] filePathsToSkipArray
                if (filePathsToSkip != null && filePathsToSkip.Length > 0)
                {
                    fileArray = fileArray.Where(filePath => !filePathsToSkip.Contains(filePath)).ToArray();
                }

                foreach (var file in fileArray)
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        string relativePathForFile = this.GetRelativePath(this.GivenSourceDirectory, fileInfo.FullName);

                        fileInfoList.Add(new FileMetaInformation(fileInfo, relativePathForFile));
                    }
                    catch (Exception)
                    {
                        throw new ArchiveErrorCodeException($"Errorcode 1. Could not process file: {file} ");
                    }
                }

                return fileInfoList;
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }
            catch (Exception)
            {
                // therse a lot of exception for Directory.GetFiles
                // this global try catch prevents weird excpetions taht could happen like for example the possibility of a directory that (it and its subdirectories) contains more than 2^32 files, which would result in an integer overflow inside that method.
                // the documentation does not mention this possibility which is kinda fishy.
                throw new ArchiveErrorCodeException($"Errorcode 1. Given directory: {this.GivenSourceDirectory} could not be processed");
            }
        }

        /// <summary>
        /// This method is used to verify the validity of the given directory source inside the class.
        /// </summary>
        public void CheckForDirectoryValidity()
        {
            if (Directory.Exists(this.GivenSourceDirectory))
            {
                this.IsSourceValid = true;

                try
                {
                    // Try to get entries in the directory.
                    Directory.GetFileSystemEntries(this.GivenSourceDirectory);
                    this.IsSourceValid = true;
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ArchiveErrorCodeException($"Directory at {this.GivenSourceDirectory} is not accessible");   
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                this.IsSourceValid = false;
            }
        }

        /// <summary>
        /// This method is used check the drive, in which the source directory resides, contains more disk space in bytes then given in the parameter.
        /// </summary>
        /// <param name="minimumRequiredSpace"> The space in bytes for which the disk should be checked.</param>
        /// <returns> A boolean indicating whether or not the disk contains more space than the given long.</returns>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown when there is not enough disk space for a new creation. Or if disk could not be checked for many reasons. </exception>
        public bool CheckForEnoughDriveSpace(long minimumRequiredSpace)
        {
            try
            {
                DriveInfo drive = new DriveInfo(this.GivenSourceDirectory);

                long availableSpace = drive.AvailableFreeSpace;

                // Define a threshold for the minimum required space. This could be a specific number or a percentage of the total drive space.
                if (availableSpace < minimumRequiredSpace)
                {
                    throw new ArchiveErrorCodeException($"Errorcode 1. Not enough space on the drive to create a new file. Required: {minimumRequiredSpace}, Available: {availableSpace}");
                }

                return true;
            }
            catch (Exception)
            {
                // probably exceptions like unauthorized, InvalidArgument, io,
                throw new ArchiveErrorCodeException($"Errorcode 1. Could not check Drive and/or Diskspace for {this.GivenSourceDirectory}! ");
            }
        }

        /// <summary>
        /// This method produces a relative path given two paths. One directly to a file, another to a directory in which the file lies in some way.
        /// </summary>
        /// <param name="directoryPath"> The directory path in which the file can be deeply buried.</param>
        /// <param name="filePath"> The path to the file for which to create a relative path for.</param>
        /// <returns> The relative path of the given file in relation to the directory.</returns>
        /// <exception cref="ArgumentException"> Is raised when the directory path is not a sub sequence of the file path.</exception>
        public string GetRelativePath(string directoryPath, string filePath)
        {
           
            if (!filePath.Contains(directoryPath))
            {
                throw new ArgumentException($"{filePath} is not in {directoryPath}");
            }

            return filePath.Substring(directoryPath.Length).TrimStart(Path.DirectorySeparatorChar);
        }
    }
}