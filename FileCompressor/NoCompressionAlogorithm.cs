using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    class NoCompressionAlgorithm : ICompressionAlgorithm
    {
        public NoCompressionAlgorithm() 
        {

        }

        //In theory a compress is just a append, because right now i dont need to compress at any other place other then 
        public void Compress(string inputOriginalFilePath, string outputArchiveFilePath)
        {
            using (var archiveFileStream = new FileStream(outputArchiveFilePath, FileMode.Append))
            {
                //TODO TRY CATCH
                //+		$exception	{"Der Prozess kann nicht auf die Datei \"C:\\Users\\Jensh\\Desktop\\Testdatein\\test.jth\" zugreifen, da sie von einem anderen Prozess verwendet wird."}	System.IO.IOException

                using (var originalFileFileStream = new FileStream(inputOriginalFilePath, FileMode.Open, FileAccess.Read))
                {

                    int standartBufferLength = 1048576; // 1MB buffer
                    var buffer = new byte[standartBufferLength];

                    ///TODO look closer as how this actually functions.
                    int bytesRead;

                    //todo remove this variable
                    long mbsRead = 0;
                    while ((bytesRead = originalFileFileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        mbsRead++;
                        archiveFileStream.Write(buffer, 0, bytesRead);
                        //////////////////////// TODO REMOVE/////////////////////
                        if (mbsRead % 100 == 0)
                        {
                            Console.WriteLine("read mbs already: " + mbsRead);
                        }
                        ////////////////////////////////////////////////////
                    }

                    //is it  even necessary to close the streams when im ALREADY use using?
                    originalFileFileStream.Close();
                }
            }
        }

        //create a class or enum or something that holds these values, so they dont lose meaning over time.
        public string CompressionTypeCalling()
        {
            return "None";
        }

        public void Decompress(string inputOriginalFilePath, string outputArchiveFilePath, long archiveDecompressionStartPoint)
        {
            throw new NotImplementedException();
        }
    }
}
