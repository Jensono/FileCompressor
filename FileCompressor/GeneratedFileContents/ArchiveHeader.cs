//-----------------------------------------------------------------------
// <copyright file="ArchiveHeader.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class describes the what information is found inside the Archiver Header in its purest form.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This class describes the what information is found inside the Archive Header in its purest form.
    /// </summary>
    public class ArchiveHeader
    {
        /// <summary>
        /// The field for the DateTime that show when the archive was created.
        /// </summary>
        private DateTime timeOfCreation;

        /// <summary>
        /// The field for the compression calling of the compression algorithm used for the files inside the archive.
        /// </summary>
        private string compressionTypeCalling;

        /// <summary>
        /// The field for the amount of files saved inside the archive.
        /// </summary>
        private int numberOfFilesInArchive;

        /// <summary>
        /// The field for the Parity Byte encoder used by this class.
        /// </summary>
        private ParitiyByteEncoder byteEncoder;

        /// <summary>
        /// The field for the size of all the files, inside of the archive, combined.
        /// </summary>
        private long sizeOfFilesCombined;

        /// <summary>
        /// The field for the  <see cref="FixedVariables"/>Fixed Variables class used inside.
        /// </summary>
        private FixedVariables fixedVariables;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveHeader"/> class. 
        /// </summary>
        /// <param name="numberOfFilesInsideDirectory"> The sum number of files that will be inside the archive.</param>
        /// <param name="compressionTypeCalling"> The calling for the compression used as a string.</param>
        /// <param name="combinedSizeForAllFiles"> The combined size of all files, in uncompressed bytes, inside the archive.</param>
        public ArchiveHeader(int numberOfFilesInsideDirectory, string compressionTypeCalling, long combinedSizeForAllFiles)
        {
            this.TimeOfCreation = DateTime.Now;
            this.SizeOfFilesCombined = combinedSizeForAllFiles;
            this.NumberOfFilesInArchive = numberOfFilesInsideDirectory;
            this.CompressionTypeCalling = compressionTypeCalling;
            this.FixedVariables = new FixedVariables();
            this.ParityByteEncoding = new ParitiyByteEncoder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveHeader"/> class. Used when appending to the archive.
        /// </summary>
        /// <param name="oldDate"> The date time the original Archive header was created.</param>
        /// <param name="numberOfFilesInsideDirectory"> The sum number of files that will be inside the archive.</param>
        /// <param name="compressionTypeCalling"> The calling for the compression used as a string.</param>
        /// <param name="combinedSizeForAllFiles"> The combined size of all files, in uncompressed bytes, inside the archive.</param>
        public ArchiveHeader(DateTime oldDate, int numberOfFilesInsideDirectory, string compressionTypeCalling, long combinedSizeForAllFiles)
        {
            this.TimeOfCreation = oldDate;
            this.SizeOfFilesCombined = combinedSizeForAllFiles;
            this.NumberOfFilesInArchive = numberOfFilesInsideDirectory;
            this.CompressionTypeCalling = compressionTypeCalling;
            this.FixedVariables = new FixedVariables();
            this.ParityByteEncoding = new ParitiyByteEncoder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveHeader"/> class. Used when reading from the archive.
        /// </summary>
        /// <param name="archiveHeaderAsBytes"> The array of bytes from which the archive header will be extracted.</param>
        public ArchiveHeader(byte[] archiveHeaderAsBytes)
        {
            // needed in all constructors
            this.FixedVariables = new FixedVariables();
            this.ParityByteEncoding = new ParitiyByteEncoder();

            // Just to make sure the expected size is also found in the given argument
            int archiveHeaderExpectedLength = this.FixedVariables.ArchiveHeaderLength;

            if (archiveHeaderAsBytes.Length != archiveHeaderExpectedLength)
            {
                throw new ArgumentException($"{archiveHeaderAsBytes} has the wrong length, it only needs to be {archiveHeaderExpectedLength} bytes long");
            }

            if (!this.ParityByteEncoding.CheckByteArrayForParityBytes(archiveHeaderAsBytes))
            {
                throw new ArchiveErrorCodeException("Errorcode 1. File isn't a Archive File or was corrupted! ");
            }

            byte[] archiveHeaderAsBytesParityRemoved = this.ParityByteEncoding.RemoveParityBytesFromArray(archiveHeaderAsBytes);

            // extract the information that should be in the header; property by property
            long ticks = BitConverter.ToInt64(archiveHeaderAsBytesParityRemoved, this.FixedVariables.ArchiveHeaderDateTimeStartByteIndex);
            this.TimeOfCreation = new DateTime(ticks);

            // another safty mechanism for the ArchiveHeader

            // skip the first 8 array entrie, take the following 10 and turn them into a array again:
            byte[] compressionCallingRaw = archiveHeaderAsBytesParityRemoved.Skip(this.fixedVariables.ArchiveHeaderCompressionTypeStartByteIndex).Take(this.FixedVariables.ArchiveHeaderLengthOfCompressionCalling).ToArray();

            // take all the bytes until it encounters the filler bytes for the header
            byte[] compressionTypeCallingAsBytes = compressionCallingRaw.TakeWhile(byt => byt != this.FixedVariables.ArchiveHeaderCompressionCallingFillerByte).ToArray();
            this.CompressionTypeCalling = Encoding.UTF8.GetString(compressionTypeCallingAsBytes);

            this.NumberOfFilesInArchive = BitConverter.ToInt32(archiveHeaderAsBytesParityRemoved, this.FixedVariables.ArchiveHeaderNumberOfFilesStartByteIndex);

            this.SizeOfFilesCombined = BitConverter.ToInt64(archiveHeaderAsBytesParityRemoved, this.FixedVariables.ArchiveHeaderSumOfFileSizeStartByteIndex);
        }

        /// <summary>
        /// Gets or sets the Parity Byte encoder used by this class.
        /// </summary>
        /// <value> The Parity Byte encoder used by this class. </value>
        public ParitiyByteEncoder ParityByteEncoding
        {
            get 
            {
                return this.byteEncoder;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ParityByteEncoding)} cannot be null!");
                }

                this.byteEncoder = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of files saved inside the archive.
        /// </summary>
        /// <value> The amount of files saved inside the archive.</value>
        public int NumberOfFilesInArchive
        {
            get
            { 
                return this.numberOfFilesInArchive;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.NumberOfFilesInArchive)} cannot be negative!");
                }

                this.numberOfFilesInArchive = value;
            }
        }

        // is set as soon as the archive is beeing created

        /// <summary>
        /// Gets or sets the DateTime that show when the archive was created.
        /// </summary>
        /// <value> The DateTime that show when the archive was created. </value>
        public DateTime TimeOfCreation
        {
            get
            {
                return this.timeOfCreation;
            }

            set 
            {
                this.timeOfCreation = value;
            }
        }

        /// <summary>
        /// Gets or sets the compression calling of the compression algorithm used for the files inside the archive.
        /// </summary>
        /// <value> The compression calling of the compression algorithm used for the files inside the archive. </value>
        public string CompressionTypeCalling
        {
            get
            {
                return this.compressionTypeCalling;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.CompressionTypeCalling)} cannot be null!");
                }

                this.compressionTypeCalling = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of all the files IN bytes, inside of the archive, combined.
        /// </summary>
        /// <value> Tor the size of all the files, inside of the archive, combined. </value>
        public long SizeOfFilesCombined
        {
            get 
            {
                return this.sizeOfFilesCombined; 
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(this.SizeOfFilesCombined)} cannot be negative!");
                }

                this.sizeOfFilesCombined = value;
            }
        }

        /// <summary>
        /// Gets or sets the  <see cref="FixedVariables"/>Fixed Variables class used inside.
        /// </summary>
        /// <value> The  <see cref="FixedVariables"/>Fixed Variables class used inside. </value>
        public FixedVariables FixedVariables
        {
            get 
            {
                return this.fixedVariables;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException($"{nameof(this.FixedVariables)} cannot be null!");
                }

                this.fixedVariables = value;
            }
        }

        // this constructor is used for appending files to a archive, and to modfiy the existing  ArchiveHeader

        /// <summary>
        /// This method returns the Archive Header as a byte array so it can be written to files.
        /// </summary>
        /// <returns> The given archive header as a byte array. First comes the <see cref="DateTime"/>(8 bytes) then the compression calling name (10 bytes with filler bytes if the name is not long enough).
        /// Then comes the number of files as a long (8 Bytes) and finally the sum number of all files uncompressed as a long (8 bytes). Then a Parity Byte encoding is used to encode the byte array further.
        /// The class used for this is <see cref="ParitiyByteEncoder"/>.</returns>
        public byte[] GetArchiveHeaderAsBytes()
        {
            // !IMPORTANT! header length is dynamically defined in the fixed variables class, if changes need to occur, first make them in that class.

            ////change this to the correct number of bytes in the header;
            byte[] archiveHeaderAsBytes = new byte[this.FixedVariables.ArchiveHeaderOriginalLength];

            long ticks = this.TimeOfCreation.Ticks;
            byte[] dateTimeBytes = BitConverter.GetBytes(ticks);

            byte[] compressionTypeAsString = Encoding.UTF8.GetBytes(this.CompressionTypeCalling);
            if (compressionTypeAsString.Length < this.FixedVariables.ArchiveHeaderLengthOfCompressionCalling)
            {
                byte[] copyOfCompressionTypeAsBytes = compressionTypeAsString;
                compressionTypeAsString = new byte[this.FixedVariables.ArchiveHeaderLengthOfCompressionCalling];
                for (int i = 0; i < compressionTypeAsString.Length; i++)
                {
                    if (i < copyOfCompressionTypeAsBytes.Length)
                    {
                        compressionTypeAsString[i] = copyOfCompressionTypeAsBytes[i];
                    }
                    else
                    {
                        compressionTypeAsString[i] = this.FixedVariables.ArchiveHeaderCompressionCallingFillerByte;
                    }
                }
            }

            byte[] numberOfFilesAsBytes = BitConverter.GetBytes(this.NumberOfFilesInArchive);

            byte[] combinedOriginalSizeOfFilesInArchive = BitConverter.GetBytes(this.SizeOfFilesCombined);

            // First 8 bytes will be copyed inside the header
            dateTimeBytes.CopyTo(archiveHeaderAsBytes, this.FixedVariables.ArchiveHeaderDateTimeStartByteIndex); // +8
            compressionTypeAsString.CopyTo(archiveHeaderAsBytes, this.FixedVariables.ArchiveHeaderCompressionTypeStartByteIndex); // +10
            numberOfFilesAsBytes.CopyTo(archiveHeaderAsBytes, this.FixedVariables.ArchiveHeaderNumberOfFilesStartByteIndex); // +4
            combinedOriginalSizeOfFilesInArchive.CopyTo(archiveHeaderAsBytes, this.FixedVariables.ArchiveHeaderSumOfFileSizeStartByteIndex);

            return this.ParityByteEncoding.AddParityBytesToByteArray(archiveHeaderAsBytes);
        }

        // for a high safty factor im just going to add the same number of bytes as the length of the array itself. first is always the the actual byte, behind it the parity byte. Parity bytes are just the number
        // from 0-255 to reach a sum of 255 from the normal byte and the parity byt
               
        /// <summary>
        /// This method is used to print a archive header to the console.
        /// </summary>
        /// <param name="archiveName"> The archives name as a string.</param>
        /// <param name="archivePath"> The path to the archive as a string.</param>
        public void PrintArchiveHeaderToConsole(string archiveName, string archivePath)
        {
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