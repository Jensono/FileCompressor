using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{

    /// <summary>
    /// // BIG ASS TODO this command needs to first create a copy of the file that needs to be appended and then delete the original file after the append happend. Otherwise the weirdest shit could happen while appending.
    /// //the destination for the create command is only the foldername and file ending eg.: archive.dat, archive.jth
    //TODO TODO check the parameter properties source and destination can not be null compression can be
    ///////OR - IF THE REQUIRED PARAMETERS ARE THERE ARE ALREADY CHECKED IN THE PROCESS OUTSIDE; BUT THEY DONT WANT THAT NORMALLY.
    /// </summary>
    class ArchiveAppendCommand :IArchiveCommand 
    {
        public string SourcePathToDirectory { get; set; }
        public string ArchiveFilePath { get; set; }


        public ArchiveAppendCommand(string sourcePathToDirectory, string archiveFilePath)
        {
            this.ArchiveFilePath = archiveFilePath;
            this.SourcePathToDirectory = sourcePathToDirectory;

        }
        private ArchiveHeader ModifyArchiveHeaderForAdditionalFiles(ArchiveHeader archiveHeader, List<FileMetaInformation> fileMetaInformationList)
        {
            DateTime modifiedArchiveHeaderDateTime = archiveHeader.TimeOfCreation;
            long modifiedArchiveSumOfFileBytes = archiveHeader.SizeOfFilesCombined + GetSumOfSizeForAllFilesCompressed(fileMetaInformationList);
            int modifiedArchiveNumberOfFiles = archiveHeader.NumberOfFilesInArchive + fileMetaInformationList.Count;
            //kinda weird hack/workaround but the first time we got this homework we didnt already know how to serialize, so im prociding as if that still is the case.
            string compressionAlgorithmUsedCalling = archiveHeader.CompressionTypeCalling;

            return new ArchiveHeader(modifiedArchiveHeaderDateTime, modifiedArchiveNumberOfFiles, compressionAlgorithmUsedCalling, modifiedArchiveSumOfFileBytes);
        }


        public long GetSumOfSizeForAllFilesCompressed(List<FileMetaInformation> fileList)
        {

            //TODO EXCEPTIONS
            long sum = 0;
            foreach (FileMetaInformation fileMetaInfo in fileList)
            {
                sum += fileMetaInfo.Length;

            }

            return sum;
        }

        public bool Execute()
        {

            ////////////////////////////// COMPRESSIONALGORITHM NEEDS TO BE READ FROM FILEHEADER VIA FILEREADER


            // IF A PERSON OVERWRITTES AN ARCHIVE it first is inside the list but can never be read resulting in an error or a loop TODO FIX!! 
            //first check if the given destionation name already exists in the directory, if it does and it is one of our own files (ArchiveHeader), delete it before continueing with the process
            // TODO WHEN OVERWRITTING a file first needs to be safed under some kind of temporary name, if while the file should be overwritten there is an error both are lost!!!!!


            //TODO TODO TODO check if the archive can even be written--> DISK SPACE
            try
            {
                ArchiveFileReader archiveFileReader = new ArchiveFileReader(this.ArchiveFilePath);
                ICompressionAlgorithm compressionAlgorithm = archiveFileReader.CompressionAlogrithmenUsed;


                DirectorySourceProcessor directorySourceProcessor = new DirectorySourceProcessor(this.SourcePathToDirectory);
                //just needed for the creation command, when creating the list it skips over all files with names inside the array
                //for now just add the archivename itself, so it doesnt try to copy itself when using this command
                string[] fileNamesToSkip = new string[] { this.ArchiveFilePath };

                List<FileMetaInformation> fileMetaInfoList = directorySourceProcessor.CreateFileMetaInfoListForDirectory(compressionAlgorithm,fileNamesToSkip );


                ArchiveHeader currentArchiveHeader = new ArchiveHeader(fileMetaInfoList.Count, compressionAlgorithm.ReturnCompressionTypeCalling(), this.GetSumOfSizeForAllFilesCompressed(fileMetaInfoList));
                ArchiveFileWriter archiveFileWriter = new ArchiveFileWriter(this.SourcePathToDirectory, this.ArchiveFilePath, compressionAlgorithm);

                ArchiveHeader newArchiveHeader = this.ModifyArchiveHeaderForAdditionalFiles(archiveFileReader.ReturnArchiveHeader(), fileMetaInfoList);

                //read the archiveheader and modify it to fit more files.

                archiveFileWriter.AppendToArchive(this.ArchiveFilePath, fileMetaInfoList, newArchiveHeader);
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
