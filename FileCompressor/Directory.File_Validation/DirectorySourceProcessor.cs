//-----------------------------------------------------------------------
// <copyright file="DirectorySourceProcessor.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class validates a given path, to make sure it is a valid directory and can extract further information form there,.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    // this class is given a source and checks if the given string is a valid directory on the machine, if so it then can create a list of fileMetaInformation about the directory and give that information back
    public class DirectorySourceProcessor
    {
        private string givenSourceDirectory;
        private bool isSourceValid;        

        public DirectorySourceProcessor(string sourceDirectory)
        {
            if (sourceDirectory is null || sourceDirectory == string.Empty)
            {
                throw new ArgumentException("Source directory must not be null or empty.");
            }

            this.GivenSourceDirectory = sourceDirectory;

            this.CheckForDirectoryValidity();
            if (!this.IsSourceValid)
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. Given Path  {sourceDirectory} does not exist ");
            }
        }

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

        // TODO WTF HAPPENS HERE maybe remove all the returning nulls

        public List<FileMetaInformation> CreateFileMetaInfoListForDirectory(ICompressionAlgorithm compressionAlgorithm, string[] filePathsToSkip)
        {
            this.CheckForDirectoryValidity();
            if (!this.IsSourceValid)
            {
                throw new ArchiveErrorCodeException("Errorcode 1. Given Source is not valid!");
            }

            List<FileMetaInformation> fileInfoList = new List<FileMetaInformation>();

            try
            {
                // WHAT THE FUCK HAPPENS IF YOU GIVE IT A DIRECTORY WITH MORE THAN int.max FILES????
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
                    catch (Exception e)
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
            catch (Exception e)
            {
                // therse a lot of exception for Directory.GetFiles
                // this global try catch prevents weird shit like for example the possibility of a directory that (it and its subdirectories) contains more than 2^32 files, which would result in an integer overflow inside that method.
                // the documentation does not mention this possibility which is kinda fishy.
                throw new ArchiveErrorCodeException($"Errorcode 1. Given directory: {this.GivenSourceDirectory} could not be processed");
            }
        }

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
                    Console.WriteLine($"Directory at {this.GivenSourceDirectory} is not accessible");
                    this.IsSourceValid = false;
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

        public string GetRelativePath(string directoryPath, string filePath)
        {
            // could be better: if (!filePath.StartsWith(directoryPath, StringComparison.OrdinalIgnoreCase)) but we never learned this i think . TODO!! ASK IN FORUM
            if (!filePath.Contains(directoryPath))
            {
                throw new ArgumentException($"{filePath} is not in {directoryPath}");
            }

            return filePath.Substring(directoryPath.Length).TrimStart(Path.DirectorySeparatorChar);
        }
    }
}