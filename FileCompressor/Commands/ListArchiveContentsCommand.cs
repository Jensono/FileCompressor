using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCompressor
{
    public class ListArchiveContentsCommand : IArchiveCommand
    {
        private string archiveSource;

        public ListArchiveContentsCommand(string source)
        {
            this.archiveSource = source;

           
        }

        public bool Execute()
        {
            if (File.Exists(this.archiveSource))
            {
                try
                {
                    ReadArchiveFileAndListEntries();
                }
                catch (ArchiveErrorCodeException e)
                {
                    //Command failed during exectuion
                    return false;
                    throw e;
                }
                
            }
            else
            {
                //TODO CONVERT TO ERROR CODE
                Console.WriteLine($"Error Code 1,{this.archiveSource} given source does not exists");
                return false;
            }
            return true;
        }

        private void ReadArchiveFileAndListEntries()
        {
            //method only utilizes the filerader
            ArchiveFileReader archiveReader = new ArchiveFileReader(this.archiveSource) ;

            List<string> entries = archiveReader.ReadArchiveFileAndReturnEntries();

            foreach (var item in entries)
            {
                Console.WriteLine(item);
            }


        }

    }
}

