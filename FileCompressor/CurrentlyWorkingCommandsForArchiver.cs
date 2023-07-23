using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileCompressor
{
   
   public  class CurrentlyWorkingCommandsForArchiver
    {
        //small todo, split this class into more componentns that make it up, but as it is its prob. easier to understands what it does.

        public CurrentlyWorkingCommandsForArchiver() { }


        


        //TODO CHECK TO SEE IF ALL THE COMMANDS HAVE DIFFRENT LONG AND SHORTNAMES; THEY NEED TO BE UNIQUE FOR THIS TO WORK
        public List<ICommandLineCommand> ReturnCurrentlyWokringCommandList()
        {
            /// Create all the functions that are used inside the ParameterInformation

            //RLE doesnt require any other arguments after it.so the array needs to be empty
            Func<string[], bool> checkForRLECopmressionParameterValidity = (parameter) =>
            {
                if (parameter.Length == 0)
                {
                    return true;
                }
                return false;
            };

            //command can only contain one string that can be parsed into an integer. The integer can only be between 1 and 10.
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
            //only lands here if the lenght of the array is one and the string is a integer between 
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

            //no additional validation can be done becouse souce and destination can be very diffrent things depending on the command, they can be filenames, directories, i could check if it is a valid path or not but the commands do that anyway.
            //also commands can have paths in them that will only be created after the command entered runtime which makes it impossible to confirm if a path is valid or not before excectuion and runtime.
            return true;

            };

            
            

            RLECompressionParameter rleCompressionParameter = new RLECompressionParameter("-rle", "--rleCompress");
            WaitTimeParameter waitTimeBetweenTriesParameter = new WaitTimeParameter("-w", "--wait");
            RetriesParameter retriesParameter = new RetriesParameter("-r", "--retry");


            SourceParameter sourceParameter = new SourceParameter("-s", "--source");
            DestinationParameter destinationParameter = new DestinationParameter("-d", "--destination");


            //create all optional List for the Command
            List<IParameter> createOptionalParameters = new List<IParameter>() { rleCompressionParameter, retriesParameter, waitTimeBetweenTriesParameter };
            List<IParameter> allOtherCommandsOptionalParameters = new List<IParameter>() { retriesParameter, waitTimeBetweenTriesParameter };
            //CREATE ALL THE REQUIRED PARAMETER LIST FOR THE COMMANDS

            List<IParameter> createAppendExtractRequiredParameters = new List<IParameter>() { destinationParameter, sourceParameter };
            List<IParameter> listInfoCommandRequiredParameters = new List<IParameter>() { sourceParameter };

            List<ICommandLineCommand> commandLineCommands = new List<ICommandLineCommand>();

            //add this shit into the actions , inside the actions we will filter
            Action<List<IParameter>> createAction = (parameter) =>
            {

                //set the default values for this action
                string sourceString = string.Empty;
                string destinationString = string.Empty;
                int waitTimeInteger = 0;
                int retriesInteger = 0;
                ICompressionAlgorithm usedCompressionType = new NoCompressionAlgorithm();
                //the required parameters must be inside becouse i already checked before, i could stillt todo implement a check to see if there are there
                foreach (IParameter param in parameter)
                {
                    switch (param)
                    {
                        case SourceParameter foundSourceParam:
                            //disgusting ass shit, there must be a better way right?  todo FIX THIS UGLY ASS SHIT CASTING
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
                        //todo specifiy, it should never happen but whatever
                        default:
                            throw new ArgumentException("The given Parameter does not exist for this command, ERRORCODE 1 TODO");

                            //break;
                    }
                }

                //todo; THIS COULD be seperated into a new class THAT JUST takes a ICommand
                

                CreateArchiveCommand createArchive = new CreateArchiveCommand(sourceString, destinationString, usedCompressionType);
               
                    for (int i = -1; i < retriesInteger; i++)
                    {
                        try
                        {   //try to execute the command and catch potential errorcodes
                            createArchive.Execute();
                            //break the loop after one succeful retry
                            break;
                        }
                        catch (ArchiveErrorCodeException e)
                        {
                            //if the last try is reached just throw the Exception upwards.
                            if (i == retriesInteger - 1)
                            {
                                throw e;
                            }
                            Thread.Sleep(1000 * waitTimeInteger);
                        }
                       
                        
                    }
                    


            };

            Action<List<IParameter>> appendAction = (parameter) =>
            {



                //set the default values for this action
                string sourceString = string.Empty;
                string destinationString = string.Empty;
                int waitTimeInteger = 0;
                int retriesInteger = 0;
                //the required parameters must be inside becouse i already checked before, i could stillt todo implement a check to see if there are there
                foreach (IParameter param in parameter)
                {
                    switch (param)
                    {
                        case SourceParameter foundSourceParam:
                            //disgusting ass shit, there must be a better way right?  todo FIX THIS UGLY ASS SHIT CASTING
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

                        //todo specifiy, it should never happen but whatever
                        default:
                            throw new ArgumentException("The given Parameter does not exist for this command, ERRORCODE 1 TODO");

                            //break;
                    }
                }

                //todo; THIS COULD be seperated into a new class THAT JUST takes a ICommand

                ArchiveAppendCommand appendArchiveCommand = new ArchiveAppendCommand(sourceString, destinationString);

                for (int i = -1; i < retriesInteger; i++)
                {
                    try
                    {
                        appendArchiveCommand.Execute();
                        //break the loop after one succeful retry
                        break;
                    }
                    catch (ArchiveErrorCodeException e)
                    {
                        //if the last try is reached just throw the Exception upwards.
                        if (i == retriesInteger - 1)
                        {
                            throw e;
                        }
                        Thread.Sleep(1000 * waitTimeInteger);
                    }


                }



            };

            Action<List<IParameter>> extractAction = (parameter) =>
            {



                //set the default values for this action
                string sourceString = string.Empty;
                string destinationString = string.Empty;
                int waitTimeInteger = 0;
                int retriesInteger = 0;
                //the required parameters must be inside becouse i already checked before, i could stillt todo implement a check to see if there are there
                foreach (IParameter param in parameter)
                {
                    switch (param)
                    {
                        case SourceParameter foundSourceParam:
                            //disgusting ass shit, there must be a better way right?  todo FIX THIS UGLY ASS SHIT CASTING
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

                        //todo specifiy, it should never happen but whatever
                        default:
                            throw new ArgumentException("The given Parameter does not exist for this command, ERRORCODE 1 TODO");

                            //break;
                    }
                }

                //todo; THIS COULD be seperated into a new class THAT JUST takes a ICommand

                ExtractArchiveCommand extractArchiveCommand = new ExtractArchiveCommand(sourceString, destinationString);

                for (int i = -1; i < retriesInteger; i++)
                {
                    try
                    {
                        extractArchiveCommand.Execute();
                        //break the loop after one succeful retry
                        break;
                    }
                    catch (ArchiveErrorCodeException e)
                    {
                        //if the last try is reached just throw the Exception upwards.
                        if (i == retriesInteger - 1)
                        {
                            throw e;
                        }
                        Thread.Sleep(1000 * waitTimeInteger);
                    }


                }

            };

            Action<List<IParameter>> infoAction = (parameter) =>
            {



                //set the default values for this action
                string sourceString = string.Empty;
                int waitTimeInteger = 0;
                int retriesInteger = 0;
                //the required parameters must be inside becouse i already checked before, i could stillt todo implement a check to see if there are there
                foreach (IParameter param in parameter)
                {
                    switch (param)
                    {
                        case SourceParameter foundSourceParam:
                            //disgusting ass shit, there must be a better way right?  todo FIX THIS UGLY ASS SHIT CASTING
                            sourceString = (string)foundSourceParam.Value;
                            break;                 

                        case RetriesParameter foundRetriesParameter:

                            retriesInteger = (int)foundRetriesParameter.Value;
                            break;

                        case WaitTimeParameter foundWaittimeParameter:

                            waitTimeInteger = (int)foundWaittimeParameter.Value;
                            break;

                        //todo specifiy, it should never happen but whatever
                        default:
                            throw new ArgumentException("The given Parameter does not exist for this command, ERRORCODE 1 TODO");

                            //break;
                    }
                }


                ArchiveInfoCommand infoArchiveCommand = new ArchiveInfoCommand(sourceString);
                for (int i = -1; i < retriesInteger; i++)
                {
                    try
                    {
                        infoArchiveCommand.Execute();
                        //break the loop after one succeful retry
                        break;
                    }
                    catch (ArchiveErrorCodeException e)
                    {
                        //if the last try is reached just throw the Exception upwards.
                        if (i == retriesInteger - 1)
                        {
                            throw e;
                        }
                        Thread.Sleep(1000 * waitTimeInteger);
                    }


                }


            };

            Action<List<IParameter>> listAction = (parameter) =>
            {
                //set the default values for this action
                string sourceString = string.Empty;
                int waitTimeInteger = 0;
                int retriesInteger = 0;
                //the required parameters must be inside becouse i already checked before, i could stillt todo implement a check to see if there are there
                foreach (IParameter param in parameter)
                {
                    switch (param)
                    {
                        case SourceParameter foundSourceParam:
                            //disgusting ass shit, there must be a better way right?  todo FIX THIS UGLY ASS SHIT CASTING
                            sourceString = (string)foundSourceParam.Value;
                            break;

                        case RetriesParameter foundRetriesParameter:

                            retriesInteger = (int)foundRetriesParameter.Value;
                            break;

                        case WaitTimeParameter foundWaittimeParameter:

                            waitTimeInteger = (int)foundWaittimeParameter.Value;
                            break;

                        //todo specifiy, it should never happen but whatever
                        default:
                            throw new ArgumentException("The given Parameter does not exist for this command, ERRORCODE 1 TODO");

                            //break;
                    }
                }


                ListArchiveContentsCommand infoArchiveCommand = new ListArchiveContentsCommand(sourceString);
                for (int i = -1; i < retriesInteger; i++)
                {
                    try
                    {
                        infoArchiveCommand.Execute();
                        //break the loop after one succeful retry
                        break;
                    }
                    catch (ArchiveErrorCodeException e)
                    {
                        //if the last try is reached just throw the Exception upwards.
                        if (i == retriesInteger - 1)
                        {
                            throw e;
                        }
                        Thread.Sleep(1000 * waitTimeInteger);
                    }


                }
            };


            CommandLineProductiveCommand createCommand = new CommandLineProductiveCommand("-c", "--create", createAction, createOptionalParameters, createAppendExtractRequiredParameters);
            CommandLineProductiveCommand appendCommand = new CommandLineProductiveCommand("-a", "--append", appendAction, allOtherCommandsOptionalParameters, createAppendExtractRequiredParameters);
            CommandLineProductiveCommand extractCommand = new CommandLineProductiveCommand("-x", "--extract", extractAction, allOtherCommandsOptionalParameters, createAppendExtractRequiredParameters);
            CommandLineProductiveCommand infoCommand = new CommandLineProductiveCommand("-i", "--info", infoAction, allOtherCommandsOptionalParameters, listInfoCommandRequiredParameters);
            CommandLineProductiveCommand listCommand = new CommandLineProductiveCommand("-l", "--list", listAction, allOtherCommandsOptionalParameters, listInfoCommandRequiredParameters);



            List<ICommandLineCommand> currentlyWorkingCommandLineArguments = new List<ICommandLineCommand>() { createCommand, appendCommand, extractCommand, infoCommand, listCommand };


            return currentlyWorkingCommandLineArguments;
            //make a list for all the commands and make it so its returnable for outside, create a new class that can take a string[] array and that then scans for commands, first all longcommandnames will be changed to short command names
            // after that the list is seperated into diffrent string[], for all ARRAYS the corresponding list of required parameters is taken as a foundation for scanning the now smaller string[]. If all required parameters are found now scan for 


            //////////////////////////////////////////////////////////////////////////////////////////////////////MOVE THIS SHIT NOW /////////////////////////////////////////////////////////////////////////////////////////////////



        }
    }
}
