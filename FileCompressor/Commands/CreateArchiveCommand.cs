using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    class CreateArchiveCommand
    {
        //source and destination, source is a path to a directory that should be compressed. Destination is just a file name that needs a ending. but maybe one should just be able to sepcify a name only
        //TODO BOTH NEED TO BE CHECKED BEFORE STARTING THE CREATION PROCESS
        //TODO THERE NEED TO BE A CHECK TO SEE IF ENOUGH DISK SPACE IS READY FOR THE ARCHIVE FILE
        
        
        public CreateArchiveCommand(string sourcePathToDirectory, string destinationNameForTheFile,bool isRleCompressionActive) 
        {

            DirectorySourceProcessor directorySourceProcessor = new DirectorySourceProcessor(sourcePathToDirectory);
            List<FileMetaInformation> fileMetaInfoList = directorySourceProcessor.CreateFileMetaInfoListForDirectory();

            ArchiveHeader currentArchiveHeader = new ArchiveHeader(fileMetaInfoList.Count,isRleCompressionActive,this.GetSumOfSizeForAllFilesCompressed(fileMetaInfoList));

            ArchiveFileWriter archiveFileWriter = new ArchiveFileWriter(sourcePathToDirectory,destinationNameForTheFile,isRleCompressionActive);
            archiveFileWriter.CreateArchive(currentArchiveHeader,fileMetaInfoList);

        }

        public long GetSumOfSizeForAllFilesCompressed(List<FileMetaInformation> fileList)
        {

            long sum = 0;
            foreach (FileMetaInformation fileMetaInfo in fileList)
            {
                sum += fileMetaInfo.FileInfo.Length;

            }

            return sum;
        }






    }
}
