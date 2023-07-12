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

        private void ReadAndConvertArchiveHeader()
        {
            byte[] buffer = new byte[21];

            try
            {
                using (FileStream fs = new FileStream(this.archiveSource, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(buffer, 0, buffer.Length);
                }
            }
            catch (Exception)
            {
                throw;
            }

            try
            {
                ArchiveHeader header = new ArchiveHeader(buffer);

                FileInfo fileInfo = new FileInfo(this.archiveSource);
                header.PrintArchiveHeaderToConsole(fileInfo.Name, fileInfo.DirectoryName);
                
            }
            //TODO BUILD OWN EXCEPTION FOR ARCHIVE HEADER IF THERE WAS DATA THAT WAS NOT EXPECTED!
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
