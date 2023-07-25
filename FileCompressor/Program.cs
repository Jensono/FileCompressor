using System;

namespace FileCompressor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //TODO TODO TODO AFTER FINAL TESTING /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //change to the args of the main method TODOTODO ///////////////
            //TODO TODO CHANGE ALL TRY-CATCH(Exceptions) to at least throw a ARCHIVEERROR not a normal one.
            while (true)
            {
                string readShit = Console.ReadLine();
                string[] FAKEARGUMENTS = readShit.Split(' ');

                CurrentlyWorkingCommandsAndCompressionsForArchiver currentlyWorkingCommandsForArchiver = new CurrentlyWorkingCommandsAndCompressionsForArchiver();
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

        }
    }
}