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
                //disk space is checked inside there
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
