using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    class CreateArchiveCommand : IArchiveCommand
    {
        //source and destination, source is a path to a directory that should be compressed. Destination is just a file name that needs a ending. but maybe one should just be able to sepcify a name only
        //TODO BOTH NEED TO BE CHECKED BEFORE STARTING THE CREATION PROCESS
        //TODO THERE NEED TO BE A CHECK TO SEE IF ENOUGH DISK SPACE IS READY FOR THE ARCHIVE FILE

        //todo null check for the compressionalgorithm
        public string SourcePathToDirectory { get; set; }
        public string DestinationNameForFile { get; set; }

        public ICompressionAlgorithm UsedCompression { get; set; }
        public CreateArchiveCommand(string sourcePathToDirectory, string destinationNameForTheFile,ICompressionAlgorithm compressionAlgorithm ) 
        {
            this.UsedCompression = compressionAlgorithm;
            this.DestinationNameForFile = destinationNameForTheFile;
            this.SourcePathToDirectory = sourcePathToDirectory;

        }

        public bool Execute()
        {

            try
            {
                DirectorySourceProcessor directorySourceProcessor = new DirectorySourceProcessor(this.SourcePathToDirectory);
                string[] filesToSkip = new string[] { Path.Combine(this.SourcePathToDirectory,this.DestinationNameForFile )};
                List<FileMetaInformation> fileMetaInfoList = directorySourceProcessor.CreateFileMetaInfoListForDirectory(this.UsedCompression,filesToSkip) ;


                ArchiveHeader currentArchiveHeader = new ArchiveHeader(fileMetaInfoList.Count, this.UsedCompression.ReturnCompressionTypeCalling(), this.GetSumOfSizeForAllFilesCompressed(fileMetaInfoList));

                ArchiveFileWriter archiveFileWriter = new ArchiveFileWriter(this.SourcePathToDirectory, this.DestinationNameForFile, this.UsedCompression);
                archiveFileWriter.CreateArchive(currentArchiveHeader, fileMetaInfoList);

            }
            catch (ArchiveErrorCodeException e)
            {
              
                throw e;
                //return false;
            }

            return true;
            // IF A PERSON OVERWRITTES AN ARCHIVE it first is inside the list but can never be read resulting in an error or a loop TODO FIX!! 
            //first check if the given destionation name already exists in the directory, if it does and it is one of our own files (ArchiveHeader), delete it before continueing with the process
            // TODO WHEN OVERWRITTING a file first needs to be safed under some kind of temporary name, if while the file should be overwritten there is an error both are lost!!!!!


            //TODO TODO TODO check if the archive can even be written--> DISK SPACE
            //this directorysourceprocessor also needs to throw the errorcode1 shit
           


        }

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
