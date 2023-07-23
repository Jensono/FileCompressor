using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCompressor
{
    public class ListArchiveContentsCommand : IArchiveCommand
    {
        //todo fields and properties
        private string ArchiveSource { get; set; }

        public ListArchiveContentsCommand(string source)
        {
            this.ArchiveSource = source;

           
        }

        public bool Execute()
        {
            if (File.Exists(this.ArchiveSource))
            {
                try
                {
                    ReadArchiveFileAndListEntries();
                }
                catch (ArchiveErrorCodeException e)
                {
                    //Command failed during exectuion
                    //return false;
                    throw e;
                }
                
            }
            else
            {
                //TODO CONVERT TO ERROR CODE
                throw new ArchiveErrorCodeException($"The file at {this.ArchiveSource} was not found. ");

                //return false;
            }
            return true;
        }

        private void ReadArchiveFileAndListEntries()
        {
            //method only utilizes the filerader
            ArchiveFileReader archiveReader = new ArchiveFileReader(this.ArchiveSource) ;

            List<string> entries = archiveReader.ReadArchiveFileAndReturnEntries();

            foreach (var item in entries)
            {
                Console.WriteLine(item);
            }


        }

    }
}

