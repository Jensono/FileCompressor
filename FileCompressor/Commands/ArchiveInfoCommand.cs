using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    class ArchiveInfoCommand : IArchiveCommand
    {

        //TODO FIELDS AND SHIT
        public string ArchiveSource { get; set; }

        public ArchiveInfoCommand(string source)
        {
            
            this.ArchiveSource = source;

            
        }

        public override bool Equals(object obj)
        {
            return obj is ArchiveInfoCommand command &&
                   ArchiveSource == command.ArchiveSource;
        }

        public bool Execute()
        {
            if (File.Exists(this.ArchiveSource))
            {
                try
                {
                    ReadAndConvertArchiveHeader();

                }
                catch (ArchiveErrorCodeException e)
                {
                    return false;
                    throw e;
                }
                
            }
            else
            {
                return false;
                //TODO CONVERT TO ERROR CODE
                Console.WriteLine("Error Code 1, given source does not exists");
                throw new FileNotFoundException($"The file at {this.ArchiveSource} was not found. ");
            }
           

            return true;
        }

        //for reading the file header we dont need a specific compressionAlogrithm as all headers are written the same, regardeless of compression

        private void ReadAndConvertArchiveHeader()
        {
            ArchiveFileReader archiveReader = new ArchiveFileReader(this.ArchiveSource);
            archiveReader.ReadArchiveHeaderAndPrintToConsole();
        }
    }
}
