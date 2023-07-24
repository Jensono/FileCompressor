using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    public class ArchiveHeader
    {
        //todo fix the fields and propeties please

        //is set as soon as the archive is beeing created
        public DateTime TimeOfCreation { get;}
        private int numberOfFilesInArchive;
        public string CompressionTypeCalling { get; set; }
        private ParitiyByteEncoder byteEncoder;
        public ParitiyByteEncoder ParityByteEncoding
        {
            get { return this.byteEncoder; }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ParityByteEncoding)} cannot be null!");
                }
                this.byteEncoder = value;
            }
        }
        public int NumberOfFilesInArchive
        {
            get { return this.numberOfFilesInArchive; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.NumberOfFilesInArchive)} cannot be negative!");
                }
                this.numberOfFilesInArchive = value;
            }
        }

        private long sizeOfFilesCombined;
        public long SizeOfFilesCombined
        {
            get { return this.sizeOfFilesCombined; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.SizeOfFilesCombined)} cannot be negative!");
                }
                this.sizeOfFilesCombined = value;
            }
        }

        private FixedVariables fixedVariables;
        public FixedVariables FixedVariables
        {
            get { return this.fixedVariables; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException($"{nameof(this.FixedVariables)} cannot be null!");
                }
                this.fixedVariables = value;
            }
        }

        

        public ArchiveHeader(int numberOfFilesInsideDirectory, string compressionTypeCalling,long combinedSizeForAllFiles)
        {

            this.TimeOfCreation = DateTime.Now;
            this.SizeOfFilesCombined = combinedSizeForAllFiles;
            this.NumberOfFilesInArchive = numberOfFilesInsideDirectory;
            this.CompressionTypeCalling = compressionTypeCalling;
            this.FixedVariables = new FixedVariables();
            this.ParityByteEncoding = new ParitiyByteEncoder();

                
        }

        //this constructor is used for appending files to a archive, and to modfiy the existing  ArchiveHeader
        public ArchiveHeader(DateTime oldDate,int numberOfFilesInsideDirectory, string compressionTypeCalling, long combinedSizeForAllFiles)
        {

            this.TimeOfCreation = oldDate;
            this.SizeOfFilesCombined = combinedSizeForAllFiles;
            this.NumberOfFilesInArchive = numberOfFilesInsideDirectory;
            this.CompressionTypeCalling = compressionTypeCalling;
            this.FixedVariables = new FixedVariables();
            this.ParityByteEncoding = new ParitiyByteEncoder();

        }

        public byte[] GetArchiveHeaderAsBytes()
        {

            // !IMPORTANT! header length is dynamically defined in the fixed variables class, if changes need to occur, first make them in that class.

            ////change this to the correct number of bytes in the header;
            byte[] archiveHeaderAsBytes = new byte[this.FixedVariables.ArchiveHeaderOriginalLength];

            long ticks = this.TimeOfCreation.Ticks;
            byte[] dateTimeBytes = BitConverter.GetBytes(ticks);





            byte[] compressionTypeAsString = Encoding.UTF8.GetBytes(this.CompressionTypeCalling);
            if (compressionTypeAsString.Length<this.FixedVariables.ArchiveHeaderLengthOfCompressionCalling)
            {
                byte[] copyOfCompressionTypeAsBytes = compressionTypeAsString;
                compressionTypeAsString = new byte[this.FixedVariables.ArchiveHeaderLengthOfCompressionCalling];
                for (int i = 0; i < compressionTypeAsString.Length; i++)
                {
                    if (i<copyOfCompressionTypeAsBytes.Length)
                    {
                        compressionTypeAsString[i] = copyOfCompressionTypeAsBytes[i];
                    }
                    else
                    {
                        //kinda retarded that i have to use the first 
                        compressionTypeAsString[i] = this.FixedVariables.ArchiveHeaderCompressionCallingFillerByte;
                    }
                }
            }

            byte[] numberOfFilesAsBytes = BitConverter.GetBytes(this.NumberOfFilesInArchive);

            byte[] combinedOriginalSizeOfFilesInArchive = BitConverter.GetBytes(this.SizeOfFilesCombined);

            
            //First 8 bytes will be copyed inside the header
            dateTimeBytes.CopyTo(archiveHeaderAsBytes, this.FixedVariables.ArchiveHeaderDateTimeStartByteIndex); //+8
            compressionTypeAsString.CopyTo(archiveHeaderAsBytes, this.FixedVariables.ArchiveHeaderCompressionTypeStartByteIndex); //+10
            numberOfFilesAsBytes.CopyTo(archiveHeaderAsBytes, this.FixedVariables.ArchiveHeaderNumberOfFilesStartByteIndex); //+4
            combinedOriginalSizeOfFilesInArchive.CopyTo(archiveHeaderAsBytes, this.FixedVariables.ArchiveHeaderSumOfFileSizeStartByteIndex);

            return this.ParityByteEncoding.AddParityBytesToByteArray(archiveHeaderAsBytes);

            

            

        }

        // for a high safty factor im just going to add the same number of bytes as the length of the array itself. first is always the the actual byte, behind it the parity byte. Parity bytes are just the number
        // from 0-255 to reach a sum of 255 from the normal byte and the parity byt 
       

        public ArchiveHeader(byte[] archiveHeaderAsBytes)
        {
            //needed in all constructors
            this.FixedVariables = new FixedVariables();
            this.ParityByteEncoding = new ParitiyByteEncoder();
            // Just to make sure the expected size is also found in the given argument
            int archiveHeaderExpectedLength = this.FixedVariables.ArchiveHeaderLength;

            if (archiveHeaderAsBytes.Length != archiveHeaderExpectedLength)
                throw new ArgumentException($"{archiveHeaderAsBytes} has the wrong length, it only needs to be {archiveHeaderExpectedLength} bytes long");


            if (!this.ParityByteEncoding.CheckByteArrayForParityBytes(archiveHeaderAsBytes))
            {
                throw new ArchiveErrorCodeException("Errorcode 1. File isn't a Archive File or was corrupted! ");
            }

            byte[] archiveHeaderAsBytesParityRemoved = this.ParityByteEncoding.RemoveParityBytesFromArray(archiveHeaderAsBytes);

            // extract the information that should be in the header; property by property
            long ticks = BitConverter.ToInt64(archiveHeaderAsBytesParityRemoved, this.FixedVariables.ArchiveHeaderDateTimeStartByteIndex);
            this.TimeOfCreation = new DateTime(ticks);

            //another safty mechanism for the ArchiveHeader

            //skip the first 8 array entrie, take the following 10 and turn them into a array again:
            byte[] compressionCallingRaw = archiveHeaderAsBytesParityRemoved.Skip(this.fixedVariables.ArchiveHeaderCompressionTypeStartByteIndex).Take(this.FixedVariables.ArchiveHeaderLengthOfCompressionCalling).ToArray();

            //take all the bytes until it encounters the filler bytes for the header
            byte[] compressionTypeCallingAsBytes = compressionCallingRaw.TakeWhile(byt => byt != this.FixedVariables.ArchiveHeaderCompressionCallingFillerByte).ToArray();
            this.CompressionTypeCalling = Encoding.UTF8.GetString(compressionTypeCallingAsBytes);       
          

            this.NumberOfFilesInArchive = BitConverter.ToInt32(archiveHeaderAsBytesParityRemoved, this.FixedVariables.ArchiveHeaderNumberOfFilesStartByteIndex);

            this.SizeOfFilesCombined = BitConverter.ToInt64(archiveHeaderAsBytesParityRemoved, this.FixedVariables.ArchiveHeaderSumOfFileSizeStartByteIndex);
        }

      

        public void PrintArchiveHeaderToConsole(string archiveName,string archivePath)
        {
            //this.TimeOfCreation = DateTime.Now;
            //this.SizeOfFilesCombined = combinedSizeForAllFiles;
            //this.NumberOfFilesInArchive = numberOfFilesInsideDirectory;
            //this.RLECompressionActive = isRLECompressionActive;
            Console.WriteLine($"Archive Name: {archiveName} ");
            Console.WriteLine($"Archive Path: {archivePath} ");
            Console.WriteLine(string.Empty);
            Console.WriteLine($"Number of files contained: { this.NumberOfFilesInArchive}");
            Console.WriteLine($"Size of File(kB): { this.SizeOfFilesCombined / 1024}");
            Console.WriteLine($"CompressionType: { this.CompressionTypeCalling}");
            Console.WriteLine($"Archive creation time: { this.TimeOfCreation}");
        }


    }
}
