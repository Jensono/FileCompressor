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
            var test = new ArchiveHeader(484,true,1278127318);
            byte[] archiveBytes = test.GetArchiveHeaderAsBytes();
            var test2 = new ArchiveHeader(archiveBytes);
        }
    }
}
