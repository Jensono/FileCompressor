using System;
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
            byte[] initialBuffer = new byte[21];

            // First try and check if the archive header is build normally, from the archive header we only need the number of items that is saved in the file.
            int numberOfFilesInFile = 0;
            try
            {
                using (FileStream fs = new FileStream(this.archiveSource, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(initialBuffer, 0, initialBuffer.Length);
                }

                ArchiveHeader header = new ArchiveHeader(initialBuffer);
                // This must be the last step !

                numberOfFilesInFile = header.NumberOfFilesInArchive;

                
            }
            //TODO FIND OUT WHAT EXCEPTIONS COULD BE THROWN AND WHY!
            catch (Exception e)
            {
                throw e;
            }

            try
            {
                long sumOfBytesReadUntilNow = 0;
                for (int i = 0; i < numberOfFilesInFile; i++)
                {

                    //but this shit here
                }


                //the first file header is read containing the length of the string name
                
                using (FileStream filestream = new FileStream(this.archiveSource, FileMode.Open, FileAccess.Read))
                {
                    byte[] stringNameSizeBuffer = new byte[4];
                    byte[] stringPathSizeBuffer = new byte[4];
                    // Skip the first 21 Bytes as these are the Archive Header
                    filestream.Read(stringNameSizeBuffer, 21, stringNameSizeBuffer.Length);
                    //Convert the size to int


                    //TODO CHECK TO SEE IF POSITIV IF NOT END THE READ!
                    int SizeOfNameInBytes = BitConverter.ToInt32(stringNameSizeBuffer,0);


                    byte[] fileNameBuffer = new byte[SizeOfNameInBytes];

                    //get the file name from the fileheader
                    filestream.Read(fileNameBuffer, 0, fileNameBuffer.Length);
                    string fileName = Encoding.UTF8.GetString(fileNameBuffer);

                    //Read the size of the pathname
                    filestream.Read(stringPathSizeBuffer, 0, stringPathSizeBuffer.Length);
                    int sizeOfPathInBytes = BitConverter.ToInt32(stringPathSizeBuffer, 0);
                    byte[] filePathBuffer = new byte[sizeOfPathInBytes];

                    //get the path name from the fileheader
                    filestream.Read(filePathBuffer, 0, filePathBuffer.Length);
                    string filePath = Encoding.UTF8.GetString(fileNameBuffer);

                    byte[] fileSizeNoCompressionBuffer = new byte[8];

                    filestream.Read(fileSizeNoCompressionBuffer, 0, fileSizeNoCompressionBuffer.Length);
                    long fileSizeNoCompression = BitConverter.ToInt64(fileSizeNoCompressionBuffer,0);


                    byte[] fileSizeWithCompressionBuffer = new byte[8];
                    filestream.Read(fileSizeWithCompressionBuffer, 0, fileSizeWithCompressionBuffer.Length);
                    long fileSizeWithCompression = BitConverter.ToInt64(fileSizeWithCompressionBuffer, 0);


                    //////////////////////////////////////////////// CONTINUE HERE ////////////////////////////
                    ///NOW sKIP THE DATA that we know the length of and repeat the process until the enf of the file is reached or we ran out of files inside the archive.


                }

             

            }
            
            catch (Exception e)
            {

                throw e;
            }
        }

    }
}

