

namespace FileCompressor
{
    using System;

    internal class Program
    {
        private static void Main(string[] args)
        {
            //TODO TODO TODO AFTER FINAL TESTING /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //change to the args of the main method TODOTODO ///////////////
            //TODO TODO CHANGE ALL TRY-CATCH(Exceptions) to at least throw a ARCHIVEERROR not a normal one.
            //TODO change all internals to public or private !
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