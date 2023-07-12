using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    class ArchiveHeader
    {
        //is set as soon as the archive is beeing created
        public DateTime TimeOfCreation { get;}
        private int numberOfFilesInArchive;
        public bool RLECompressionActive { get; set; }
        
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


        public ArchiveHeader(int numberOfFilesInsideDirectory, bool isRLECompressionActive,long combinedSizeForAllFiles)
        {

            this.TimeOfCreation = DateTime.Now;
            this.SizeOfFilesCombined = combinedSizeForAllFiles;
            this.NumberOfFilesInArchive = numberOfFilesInsideDirectory;
            this.RLECompressionActive = isRLECompressionActive;
            

                
        }

        public byte[] GetArchiveHeaderAsBytes()
        {

            //21 bytes - 8 for creation time, 1 for compression active or not, 

            // HOW TO CONVERT FROM DATETIME TO BYTES AND BACK
            ////var now = DateTime.Now;
            ////long ticks = now.Ticks;
            ////byte[] bytes = BitConverter.GetBytes(ticks);

            ////var ticksrecorder = BitConverter.ToInt64(bytes, 0);
            ////var then = DateTime.FromBinary(ticksrecorder);
            ////Console.WriteLine(then.ToString());


            //change this to the correct number of bytes in the header;
            byte[] archiveHeaderAsBytes = new byte[21];

            long ticks = this.TimeOfCreation.Ticks;
            byte[] dateTimeBytes = BitConverter.GetBytes(ticks);
            


            byte[] rleCompressionBoolAsByte = BitConverter.GetBytes(this.RLECompressionActive);

            byte[] numberOfFilesAsBytes = BitConverter.GetBytes(this.NumberOfFilesInArchive);

            byte[] combinedOriginalSizeOfFilesInArchive = BitConverter.GetBytes(this.SizeOfFilesCombined);

            //TODO the start index for these could in theory be saved inside another object that is created when starting the application.
            // with that it is ensured that if one wants to change the byte order or add new features to the archive header its easyily relisable.
            //First 8 bytes will be copyed inside the header
            dateTimeBytes.CopyTo(archiveHeaderAsBytes, 0);
            rleCompressionBoolAsByte.CopyTo(archiveHeaderAsBytes, 8);
            numberOfFilesAsBytes.CopyTo(archiveHeaderAsBytes, 9);
            combinedOriginalSizeOfFilesInArchive.CopyTo(archiveHeaderAsBytes, 13);

            return archiveHeaderAsBytes;

        }

        public ArchiveHeader(byte[] archiveHeaderAsBytes)
        {
            // Just to make sure the expected size is also found in the given argument
            int archiveHeaderExpectedLength = 21;

            if (archiveHeaderAsBytes.Length != archiveHeaderExpectedLength)
                throw new ArgumentException($"{archiveHeaderAsBytes} has the wrong length, it only needs to be {archiveHeaderExpectedLength} bytes long");

            // extract the information that should be in the header; property by property
            long ticks = BitConverter.ToInt64(archiveHeaderAsBytes, 0);
            this.TimeOfCreation = new DateTime(ticks);

            this.RLECompressionActive = BitConverter.ToBoolean(archiveHeaderAsBytes, 8);

            this.NumberOfFilesInArchive = BitConverter.ToInt32(archiveHeaderAsBytes, 9);

            this.SizeOfFilesCombined = BitConverter.ToInt64(archiveHeaderAsBytes, 13);
        }
    }
}
