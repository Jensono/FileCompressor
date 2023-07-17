using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{   
        public interface ICompressionAlgorithm 
        {
            void Compress(string inputOriginalFilePath, string outputArchiveFilePath);
            void Decompress(string inputOriginalFilePath, string outputArchiveFilePath, long archiveDecompressionStartPoint);

        string CompressionTypeCalling();

        }
    
}
