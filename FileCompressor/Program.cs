﻿//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class marks the entry for the programm. From here all the commands will be analyst and, if valid executed.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;

    /// <summary>
    /// This class marks the entry for the program. From here all the commands will be analyst and, if valid executed.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main method and entry point for the application.
        /// </summary>
        /// <param name="args"> The arguments given in the command line, when executing the application from the console.</param>
        private static void Main(string[] args)
        {
            // TODO TODO TODO AFTER FINAL TESTING /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // change to the args of the main method TODOTODO ///////////////
            // TODO TODO CHANGE ALL TRY-CATCH(Exceptions) to at least throw a ARCHIVEERROR not a normal one.
            // TODO change all internals to public or private !
            // check all the todos!
            // todo PRINT HELP COMMAND IF ARCHIVE ERROR HAPPENDED AND ITS USER FOULT
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