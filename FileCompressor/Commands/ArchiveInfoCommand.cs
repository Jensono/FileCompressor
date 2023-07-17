using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    class ArchiveInfoCommand
    {

        //TODO FIELDS AND SHIT
        private string archiveSource;

        public ArchiveInfoCommand(string source)
        {
            
            this.archiveSource = source;

            if (File.Exists(this.archiveSource))
            {
                ReadAndConvertArchiveHeader();
            }
            else
            {
                //TODO CONVERT TO ERROR CODE
                Console.WriteLine("Error Code 1, given source does not exists");
                throw new FileNotFoundException($"The file at {this.archiveSource} was not found. ");
            }
        }

        //for reading the file header we dont need a specific compressionAlogrithm as all headers are written the same, regardeless of compression

        private void ReadAndConvertArchiveHeader()
        {
            ArchiveFileReader archiveReader = new ArchiveFileReader(this.archiveSource,new NoCompressionAlgorithm());
            archiveReader.ReadArchiveHeaderAndPrintToConsole();
        }
    }
}
