using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    class Program
    {
        static void Main(string[] args)
        {

            CreateArchiveCommand createArchiveCommand = new CreateArchiveCommand(@"C:\Users\Jensh\Desktop\Testdatein", "test.jth", new NoCompressionAlgorithm());

            ArchiveInfoCommand archiveInfoCommand = new ArchiveInfoCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth");

            ListArchiveContentsCommand listArchiveContentsCommand = new ListArchiveContentsCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth");

            ExtractArchiveCommand extract = new ExtractArchiveCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth", @"C:\Users\Jensh\Desktop\new", new NoCompressionAlgorithm());
            //var test = new ArchiveHeader(484,true,1278127318);
            //byte[] archiveBytes = test.GetArchiveHeaderAsBytes();
            //var test2 = new ArchiveHeader(archiveBytes);
        }
    }
}
