using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    class Program
    {
        static void Main(string[] args)
        {

            //change to the args of the main method TODOTODO ///////////////
            while (true)
            {

                string readShit = Console.ReadLine();
                string[] FAKEARGUMENTS = readShit.Split(' ');

                CurrentlyWorkingCommandsForArchiver currentlyWorkingCommandsForArchiver = new CurrentlyWorkingCommandsForArchiver();
                try
                {

                    CommandLineArgumentParser parser = new CommandLineArgumentParser(currentlyWorkingCommandsForArchiver.ReturnCurrentlyWokringCommandList(), FAKEARGUMENTS);

                    parser.ParseCommandsAndExecute();


                }
                catch (ArchiveErrorCodeException e)
                {

                    Console.WriteLine(e.ErrorCode);
                }
            }


            ////testing the final CommandLineProductiveCommand

            ////CreateArchiveCommand createArchiveCommand = new CreateArchiveCommand(@"C:\Users\Jensh\Desktop\Testdatein", "test.jth", new NoCompressionAlgorithm());

            ////CreateArchiveCommand createArchiveCommand = new CreateArchiveCommand(@"C:\Users\Jensh\Desktop\Testdatein", "test.jth", new RLECompressionAlgorithm()) ;

            //ArchiveInfoCommand archiveInfoCommand = new ArchiveInfoCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth");

            //ListArchiveContentsCommand listArchiveContentsCommand = new ListArchiveContentsCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth");

            //ExtractArchiveCommand extract = new ExtractArchiveCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth", @"C:\Users\Jensh\Desktop\new");

            //ArchiveAppendCommand appendCommandFirstTry = new ArchiveAppendCommand(@"C:\Users\Jensh\Desktop\Testdatein_Extra", @"C:\Users\Jensh\Desktop\Testdatein\test.jth");

            //ArchiveInfoCommand archiveInfoCommand2 = new ArchiveInfoCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth");

            //ListArchiveContentsCommand listArchiveContentsCommand2 = new ListArchiveContentsCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth");

            //ExtractArchiveCommand extract2 = new ExtractArchiveCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth", @"C:\Users\Jensh\Desktop\appendoutput");






            //var test = new ArchiveHeader(484,true,1278127318);
            //byte[] archiveBytes = test.GetArchiveHeaderAsBytes();
            //var test2 = new ArchiveHeader(archiveBytes);
        }
    }
}
