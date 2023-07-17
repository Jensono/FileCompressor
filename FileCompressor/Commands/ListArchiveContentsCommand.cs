using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCompressor
{
    public class ListArchiveContentsCommand
    {
        private string archiveSource;

        public ListArchiveContentsCommand(string source)
        {
            this.archiveSource = source;

            if (File.Exists(this.archiveSource))
            {
                ReadArchiveFileAndListEntries();
            }
            else
            {
                //TODO CONVERT TO ERROR CODE
                Console.WriteLine("Error Code 1, given source does not exists");
                throw new FileNotFoundException($"The file at {this.archiveSource} was not found. ");
            }
        }

        private void ReadArchiveFileAndListEntries()
        {
            //method only utilizes the filerader
            ArchiveFileReader archiveReader = new ArchiveFileReader(this.archiveSource, new NoCompressionAlgorithm()) ;

            List<string> entries = archiveReader.ReadArchiveFileAndReturnEntries();

            foreach (var item in entries)
            {
                Console.WriteLine(item);
            }


        }

    }
}

