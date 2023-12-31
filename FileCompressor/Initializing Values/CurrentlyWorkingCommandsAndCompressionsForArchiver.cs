﻿//-----------------------------------------------------------------------
// <copyright file="CurrentlyWorkingCommandsAndCompressionsForArchiver.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class initializes all currently working compression types and Commands that can be used with this programm. If any new commands should be added to the application, add them here first.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// This class is used to initializes all the commands that are currently functioning for the program. All in one place, can be easily changed to different actions and validations.
    /// Available compression algorithms are also specified here.
    /// </summary>
    public class CurrentlyWorkingCommandsAndCompressionsForArchiver
    {
        // small todo ok, split this class into more componentns that make it up, but as it is its prob. easier to understands what it does.

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentlyWorkingCommandsAndCompressionsForArchiver"/> class.
        /// </summary>
        public CurrentlyWorkingCommandsAndCompressionsForArchiver()
        {
        }

        /// <summary>
        /// This method returns the currently working Compression algorithms for the program.
        /// </summary>
        /// <returns> A list of <see cref="ICompressionAlgorithm"/> that only contain algorithms which can be used in the program right now.</returns>
        public List<ICompressionAlgorithm> ReturnCurrentlyWorkingCompressionAlgorithms()
        {
            NoCompressionAlgorithm noCompressionAlgorithm = new NoCompressionAlgorithm();
            RLECompressionAlgorithm rleCompressionAlgorithm = new RLECompressionAlgorithm();

            return new List<ICompressionAlgorithm> { noCompressionAlgorithm, rleCompressionAlgorithm };
        }

        /// <summary>
        /// This method returns all the commands that are currently working for a application. It first sets all the parameters needed for the command inside this class the returns the list.
        /// </summary>
        /// <returns> The currently usable commands as a list of <see cref="ICommandLineCommand"/>. </returns>
        public List<ICommandLineCommand> ReturnCurrentlyWokringCommandList()
        {
            // Create all the functions that are used inside the ParameterInformation

            // RLE doesnt require any other arguments after it.so the array needs to be empty
            Func<string[], bool> checkForRLECopmressionParameterValidity = (parameter) =>
            {
                if (parameter.Length == 0)
                {
                    return true;
                }

                return false;
            };

            // command can only contain one string that can be parsed into an integer. The integer can only be between 1 and 10.
            Func<string[], bool> checkForWaitAndRetryParameterValidity = (parameter) =>
            {
                if (parameter.Length >= 2)
                {
                    return false;
                }

                // if there is no extra argument then its also valid, taking the value of 1
                if (parameter.Length == 0)
                {
                    return true;
                }

                int potentialRepeatArgument;
                if (!int.TryParse(parameter[0], out potentialRepeatArgument))
                {
                    return false;
                }

                if (potentialRepeatArgument < 0 || potentialRepeatArgument > 10)
                {
                    return false;
                }

                // only lands here if the lenght of the array is one and the string is a integer between
                return true;
            };

            Func<string[], bool> checkForSourceDestinationParameterValidity = (parameter) =>
            {
                if (parameter.Length >= 2)
                {
                    return false;
                }

                if (parameter.Length == 0)
                {
                    return false;
                }

                // no additional validation can be done becouse souce and destination can be very diffrent things depending on the command, they can be filenames, directories, i could check if it is a valid path or not but the commands do that anyway.
                // also commands can have paths in them that will only be created after the command entered runtime which makes it impossible to confirm if a path is valid or not before excectuion and runtime.
                return true;
            };

            RLECompressionParameter rleCompressionParameter = new RLECompressionParameter("-rle", "--rleCompress");
            WaitTimeParameter waitTimeBetweenTriesParameter = new WaitTimeParameter("-w", "--wait");
            RetriesParameter retriesParameter = new RetriesParameter("-r", "--retry");

            SourceParameter sourceParameter = new SourceParameter("-s", "--source");
            DestinationParameter destinationParameter = new DestinationParameter("-d", "--destination");

            // create all optional List for the Command
            List<IParameter> createOptionalParameters = new List<IParameter>() { rleCompressionParameter, retriesParameter, waitTimeBetweenTriesParameter };
            List<IParameter> allOtherCommandsOptionalParameters = new List<IParameter>() { retriesParameter, waitTimeBetweenTriesParameter };
            List<IParameter> helpCommandOptionalParameters = new List<IParameter>() { };

            // create all required parameter list for the commands
            List<IParameter> createAppendExtractRequiredParameters = new List<IParameter>() { destinationParameter, sourceParameter };
            List<IParameter> listInfoCommandRequiredParameters = new List<IParameter>() { sourceParameter };
            List<IParameter> helpCommandRequiredParameters = new List<IParameter>() { };

            List<ICommandLineCommand> commandLineCommands = new List<ICommandLineCommand>();
           
            Action<List<IParameter>> createAction = (parameter) =>
            {
                // set the default values for this action
                string sourceString = string.Empty;
                string destinationString = string.Empty;
                int waitTimeInteger = 1;
                int retriesInteger = 0;
                ICompressionAlgorithm usedCompressionType = new NoCompressionAlgorithm();

                foreach (IParameter param in parameter)
                {
                    switch (param)
                    {
                        case SourceParameter foundSourceParam:
                            //todo there needs to be a better way to do this without casting, this switch case is also not optiomal for extension of the program if there were ever other parameters.
                            sourceString = (string)foundSourceParam.Value;
                            break;

                        case DestinationParameter foundDestinationParameter:

                            destinationString = (string)foundDestinationParameter.Value;
                            break;

                        case RetriesParameter foundRetriesParameter:

                            retriesInteger = (int)foundRetriesParameter.Value;
                            break;

                        case WaitTimeParameter foundWaittimeParameter:

                            waitTimeInteger = (int)foundWaittimeParameter.Value;
                            break;

                        case RLECompressionParameter foundRLECompressionParameter:

                            usedCompressionType = (ICompressionAlgorithm)foundRLECompressionParameter.Value;
                            break;

                        default:
                            throw new ArchiveErrorCodeException("Errorcode 1. Given Parameter does not exist for this command.");

                            // break;
                    }
                }

                // if any of the required parameters were not intialized then throw a archive error
                if (sourceString.Equals(string.Empty) || destinationString.Equals(string.Empty))
                {
                    throw new ArchiveErrorCodeException("Errorcode 1. Required Parameters were not given for this command.");
                }

                CreateArchiveCommand createArchive = new CreateArchiveCommand(sourceString, destinationString, usedCompressionType);

                for (int i = -1; i < retriesInteger; i++)
                {
                    try
                    {   // try to execute the command and catch potential errorcodes
                        createArchive.Execute();

                        // break the loop after one succeful retry
                        break;
                    }
                    catch (ArchiveErrorCodeException e)
                    {
                        // if the last try is reached just throw the Exception upwards.
                        if (i == retriesInteger - 1)
                        {
                            throw e;
                        }

                        Console.WriteLine($"Retrying Create Command for Source: {sourceString} , Destination: {destinationString}. Encountered Error: {e.ErrorCode}");
                        Thread.Sleep(1000 * waitTimeInteger);
                    }
                }
            };

            Action<List<IParameter>> appendAction = (parameter) =>
            {
                // set the default values for this action
                string sourceString = string.Empty;
                string destinationString = string.Empty;
                int waitTimeInteger = 1;
                int retriesInteger = 0;

                foreach (IParameter param in parameter)
                {
                    switch (param)
                    {
                        case SourceParameter foundSourceParam:
                            //todo there needs to be a better way to do this without casting, this switch case is also not optiomal for extension of the program if there were ever other parameters.
                            sourceString = (string)foundSourceParam.Value;
                            break;

                        case DestinationParameter foundDestinationParameter:

                            destinationString = (string)foundDestinationParameter.Value;
                            break;

                        case RetriesParameter foundRetriesParameter:

                            retriesInteger = (int)foundRetriesParameter.Value;
                            break;

                        case WaitTimeParameter foundWaittimeParameter:

                            waitTimeInteger = (int)foundWaittimeParameter.Value;
                            break;

                        default:
                            throw new ArchiveErrorCodeException("Errorcode 1. Given Parameter does not exist for this command.");
                    }
                }

                // if any of the required parameters were not intialized then throw a archive error
                if (sourceString.Equals(string.Empty) || destinationString.Equals(string.Empty))
                {
                    throw new ArchiveErrorCodeException("Errorcode 1. Required Parameters were not given for this command.");
                }

                ArchiveAppendCommand appendArchiveCommand = new ArchiveAppendCommand(sourceString, destinationString);

                for (int i = -1; i < retriesInteger; i++)
                {
                    try
                    {
                        appendArchiveCommand.Execute();

                        // break the loop after one succeful retry
                        break;
                    }
                    catch (ArchiveErrorCodeException e)
                    {
                        // if the last try is reached just throw the Exception upwards.
                        if (i == retriesInteger - 1)
                        {
                            throw e;
                        }

                        Console.WriteLine($"Retrying Append Command for Source: {sourceString} , Destination: {destinationString}. Encountered Error: {e.ErrorCode}");
                        Thread.Sleep(1000 * waitTimeInteger);
                    }
                }
            };

            Action<List<IParameter>> extractAction = (parameter) =>
            {
                // set the default values for this action
                string sourceString = string.Empty;
                string destinationString = string.Empty;
                int waitTimeInteger = 1;
                int retriesInteger = 0;

                foreach (IParameter param in parameter)
                {
                    switch (param)
                    {
                        case SourceParameter foundSourceParam:
                            //todo there needs to be a better way to do this without casting, this switch case is also not optiomal for extension of the program if there were ever other parameters.
                            sourceString = (string)foundSourceParam.Value;
                            break;

                        case DestinationParameter foundDestinationParameter:

                            destinationString = (string)foundDestinationParameter.Value;
                            break;

                        case RetriesParameter foundRetriesParameter:

                            retriesInteger = (int)foundRetriesParameter.Value;
                            break;

                        case WaitTimeParameter foundWaittimeParameter:

                            waitTimeInteger = (int)foundWaittimeParameter.Value;
                            break;

                        default:
                            throw new ArchiveErrorCodeException("Errorcode 1. Given Parameter does not exist for this command.");

                            // break;
                    }
                }

                // if any of the required parameters were not intialized then throw a archive error
                if (sourceString.Equals(string.Empty) || destinationString.Equals(string.Empty))
                {
                    throw new ArchiveErrorCodeException("Errorcode 1. Required Parameters were not given for this command.");
                }

                ExtractArchiveCommand extractArchiveCommand = new ExtractArchiveCommand(sourceString, destinationString);

                for (int i = -1; i < retriesInteger; i++)
                {
                    try
                    {
                        extractArchiveCommand.Execute();

                        // break the loop after one succeful retry
                        break;
                    }
                    catch (ArchiveErrorCodeException e)
                    {
                        // if the last try is reached just throw the Exception upwards.
                        if (i == retriesInteger - 1)
                        {
                            throw e;
                        }

                        Console.WriteLine($"Retrying Extraction Command for Source: {sourceString} , Destination: {destinationString}. Encountered Error: {e.ErrorCode}");
                        Thread.Sleep(1000 * waitTimeInteger);
                    }
                }
            };

            Action<List<IParameter>> infoAction = (parameter) =>
            {
                // set the default values for this action
                string sourceString = string.Empty;
                int waitTimeInteger = 1;
                int retriesInteger = 0;
                foreach (IParameter param in parameter)
                {
                    switch (param)
                    {
                        case SourceParameter foundSourceParam:
                            //todo there needs to be a better way to do this without casting, this switch case is also not optiomal for extension of the program if there were ever other parameters.
                            sourceString = (string)foundSourceParam.Value;
                            break;

                        case RetriesParameter foundRetriesParameter:

                            retriesInteger = (int)foundRetriesParameter.Value;
                            break;

                        case WaitTimeParameter foundWaittimeParameter:

                            waitTimeInteger = (int)foundWaittimeParameter.Value;
                            break;

                        default:
                            throw new ArchiveErrorCodeException("Errorcode 1. Given Parameter does not exist for this command.");

                            // break;
                    }
                }

                // if any of the required parameters were not intialized then throw a archive error
                if (sourceString.Equals(string.Empty))
                {
                    throw new ArchiveErrorCodeException("Errorcode 1. Required Parameters were not given for this command.");
                }

                ArchiveInfoCommand infoArchiveCommand = new ArchiveInfoCommand(sourceString);
                for (int i = -1; i < retriesInteger; i++)
                {
                    try
                    {
                        infoArchiveCommand.Execute();

                        // break the loop after one succeful retry
                        break;
                    }
                    catch (ArchiveErrorCodeException e)
                    {
                        // if the last try is reached just throw the Exception upwards.
                        if (i == retriesInteger - 1)
                        {
                            throw e;
                        }

                        Console.WriteLine($"Retrying Info Command for Source: {sourceString} . Encountered Error: {e.ErrorCode}");
                        Thread.Sleep(1000 * waitTimeInteger);
                    }
                }
            };

            Action<List<IParameter>> listAction = (parameter) =>
            {
                // set the default values for this action
                string sourceString = string.Empty;
                int waitTimeInteger = 1;
                int retriesInteger = 0;
                foreach (IParameter param in parameter)
                {
                    switch (param)
                    {
                        case SourceParameter foundSourceParam:
                            //todo there needs to be a better way to do this without casting, this switch case is also not optiomal for extension of the program if there were ever other parameters.
                            sourceString = (string)foundSourceParam.Value;
                            break;

                        case RetriesParameter foundRetriesParameter:

                            retriesInteger = (int)foundRetriesParameter.Value;
                            break;

                        case WaitTimeParameter foundWaittimeParameter:

                            waitTimeInteger = (int)foundWaittimeParameter.Value;
                            break;

                        default:
                            throw new ArchiveErrorCodeException("Errorcode 1. Given Parameter does not exist for this command.");
                    }
                }

                // if any of the required parameters were not intialized then throw a archive error
                if (sourceString.Equals(string.Empty))
                {
                    throw new ArchiveErrorCodeException("Errorcode 1. Required Parameters were not given for this command.");
                }

                ListArchiveContentsCommand infoArchiveCommand = new ListArchiveContentsCommand(sourceString);
                for (int i = -1; i < retriesInteger; i++)
                {
                    try
                    {
                        infoArchiveCommand.Execute();
                        // break the loop after one succeful retry
                        break;
                    }
                    catch (ArchiveErrorCodeException e)
                    {
                        // if the last try is reached just throw the Exception upwards.
                        if (i == retriesInteger - 1)
                        {
                            throw e;
                        }

                        Console.WriteLine($"Retrying List Command for Source: {sourceString} . Encountered Error: {e.ErrorCode}");
                        Thread.Sleep(1000 * waitTimeInteger);
                    }
                }
            };

            Action<List<IParameter>> helpAction = (parameter) =>
            {
                foreach (IParameter param in parameter)
                {
                    switch (param)
                    {
                        default:
                            throw new ArchiveErrorCodeException("Errorcode 1. Given Parameter does not exist for this command.");

                            // break;
                    }
                }

                ArchiveHelpCommand archiveHelpCommand = new ArchiveHelpCommand();
                archiveHelpCommand.Execute();
            };

            CommandLineProductiveCommand createCommand = new CommandLineProductiveCommand("-c", "--create", createAction, createOptionalParameters, createAppendExtractRequiredParameters);
            CommandLineProductiveCommand appendCommand = new CommandLineProductiveCommand("-a", "--append", appendAction, allOtherCommandsOptionalParameters, createAppendExtractRequiredParameters);
            CommandLineProductiveCommand extractCommand = new CommandLineProductiveCommand("-x", "--extract", extractAction, allOtherCommandsOptionalParameters, createAppendExtractRequiredParameters);
            CommandLineProductiveCommand infoCommand = new CommandLineProductiveCommand("-i", "--info", infoAction, allOtherCommandsOptionalParameters, listInfoCommandRequiredParameters);
            CommandLineProductiveCommand listCommand = new CommandLineProductiveCommand("-l", "--list", listAction, allOtherCommandsOptionalParameters, listInfoCommandRequiredParameters);
            CommandLineProductiveCommand helpCommand = new CommandLineProductiveCommand("-h", "--help", helpAction, helpCommandOptionalParameters, helpCommandRequiredParameters);

            // todo ok  check to see if the short and long commandnames are all unique to ONE Command!
            List<ICommandLineCommand> currentlyWorkingCommandLineArguments = new List<ICommandLineCommand>() { createCommand, appendCommand, extractCommand, infoCommand, listCommand, helpCommand };

            return currentlyWorkingCommandLineArguments;
        }
    }
}