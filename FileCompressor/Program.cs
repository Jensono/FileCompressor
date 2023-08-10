//-----------------------------------------------------------------------
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
            FixedVariables fixedVariables = new FixedVariables();
            CurrentlyWorkingCommandsAndCompressionsForArchiver currentlyWorkingCommandsForArchiver = new CurrentlyWorkingCommandsAndCompressionsForArchiver();
            try
            {
                CommandLineArgumentParser parser = new CommandLineArgumentParser(currentlyWorkingCommandsForArchiver.ReturnCurrentlyWokringCommandList(), args);

                parser.ParseCommandsAndExecute();
            }
            catch (ArchiveErrorCodeException e)
            {
                Console.WriteLine(e.ErrorCode);

                // writes the help command to the console so the user understands how to use the application.
                Console.WriteLine(fixedVariables.HelpCommandString);
            }
        }
    }
}