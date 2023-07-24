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
                    
                    throw e;

                    //return false;
                }
                
            }
            else

            { //TODO CONVERT TO ERROR CODE

                throw new ArchiveErrorCodeException($"The file at {this.ArchiveSource} was not found. ");

                ////return false;
            }
           

            return true;
        }

        //for reading the file header we dont need a specific compressionAlogrithm as all headers are written the same, regardeless of compression

        private void ReadAndConvertArchiveHeader()
        {
            try
            {
                ArchiveFileReader archiveReader = new ArchiveFileReader(this.ArchiveSource);
                archiveReader.ReadArchiveHeaderAndPrintToConsole();
            }
            catch (ArchiveErrorCodeException e)
            {

                throw e;
            }
            
        }
    }
}
