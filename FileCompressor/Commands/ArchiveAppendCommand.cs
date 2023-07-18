using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    class ArchiveAppendCommand
    {

        public ArchiveAppendCommand(string sourcePathToDirectory, string archiveFilePath)
        {
            ////////////////////////////// COMPRESSIONALGORITHM NEEDS TO BE READ FROM FILEHEADER VIA FILEREADER


            // IF A PERSON OVERWRITTES AN ARCHIVE it first is inside the list but can never be read resulting in an error or a loop TODO FIX!! 
            //first check if the given destionation name already exists in the directory, if it does and it is one of our own files (ArchiveHeader), delete it before continueing with the process
            // TODO WHEN OVERWRITTING a file first needs to be safed under some kind of temporary name, if while the file should be overwritten there is an error both are lost!!!!!


            //TODO TODO TODO check if the archive can even be written--> DISK SPACE
           
            ArchiveFileReader archiveFileReader = new ArchiveFileReader(archiveFilePath);
            ICompressionAlgorithm compressionAlgorithm = archiveFileReader.CompressionAlogrithmenUsed;


                 DirectorySourceProcessor directorySourceProcessor = new DirectorySourceProcessor(sourcePathToDirectory);
            List<FileMetaInformation> fileMetaInfoList = directorySourceProcessor.CreateFileMetaInfoListForDirectory(compressionAlgorithm);


            ArchiveHeader currentArchiveHeader = new ArchiveHeader(fileMetaInfoList.Count, compressionAlgorithm.CompressionTypeCalling(), this.GetSumOfSizeForAllFilesCompressed(fileMetaInfoList));
            ArchiveFileWriter archiveFileWriter = new ArchiveFileWriter(sourcePathToDirectory, archiveFilePath, compressionAlgorithm);

            ArchiveHeader newArchiveHeader = this.ModifyArchiveHeaderForAdditionalFiles(archiveFileReader.ReturnArchiveHeader(), fileMetaInfoList);
            
            //read the archiveheader and modify it to fit more files.

            archiveFileWriter.AppendToArchive(archiveFilePath, fileMetaInfoList, newArchiveHeader);



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
    }
}
