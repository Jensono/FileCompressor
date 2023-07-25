

namespace FileCompressor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
                    this.ReadArchiveFileAndListEntries();
                }
                catch (ArchiveErrorCodeException e)
                {
                    throw e;
                }
            }
            else
            {
                throw new ArchiveErrorCodeException($"Errorcode 1. The file at {this.ArchiveSource} was not found. ");

                // return false;
            }
            return true;
        }

        private void ReadArchiveFileAndListEntries()
        {
            // method only utilizes the filerader
            List<string> entries = new List<string>();

            try
            {
                ArchiveFileReader archiveReader = new ArchiveFileReader(this.ArchiveSource);

                entries = archiveReader.ReadArchiveFileAndReturnEntries();
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }

            foreach (var item in entries)
            {
                Console.WriteLine(item);
            }
        }
    }
}