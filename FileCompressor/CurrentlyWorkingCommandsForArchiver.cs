using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
   
    class CurrentlyWorkingCommandsForArchiver
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

            //we look what parameterinformation matches the entries (starting at the second entry = 1 ) of the second layer list and look at the first element in the string array. This must now contain the small name parameter argument.
            //we split the rest of the parameter arguments and send them to the Function that validates a parameters arguments.If any of the parameter specifiers are wrong we send Errorcode 1.
            //if the parameter was validated we can put it in the commandparameters,or we could do that after the whole second layer list was validated.
            //BUT STILL BIUG QUESTION NOW how do we know in which order to put the parameters into the CommandParameterCalss. Do we just make fields inside the class that checks these and fill them with the values as the parameter specifications are verfied?
            // this would make the whole process not modular and very static. If any new Command was added to the system the parser would need to be updated.
            

            OptionalParameterInformation optionalRLECompressionType = new OptionalParameterInformation("-rle", "--rleCompress", checkForRLECopmressionParameterValidity);
            OptionalParameterInformation optionalWaitTimeBetweenTriesType = new OptionalParameterInformation("-w", "--wait", checkForWaitAndRetryParameterValidity);
            OptionalParameterInformation optionalRetriesForCommandType = new OptionalParameterInformation("-r", "--retry", checkForWaitAndRetryParameterValidity);


            RequiredParameterInformation requiredSource = new RequiredParameterInformation("-s", "--source", checkForSourceDestinationParameterValidity);
            RequiredParameterInformation requiredDestination = new RequiredParameterInformation("-d", "--destination", checkForSourceDestinationParameterValidity);


            //create all optional List for the Command
            List<OptionalParameterInformation> createOptionalParameters = new List<OptionalParameterInformation>() { optionalRLECompressionType, optionalRetriesForCommandType, optionalWaitTimeBetweenTriesType };
            List<OptionalParameterInformation> allOtherCommandsOptionalParameters = new List<OptionalParameterInformation>() { optionalRetriesForCommandType, optionalWaitTimeBetweenTriesType };
            //CREATE ALL THE REQUIRED PARAMETER LIST FOR THE COMMANDS

            List<RequiredParameterInformation> createAppendExtractRequiredParameters = new List<RequiredParameterInformation>() { requiredDestination, requiredSource };
            List<RequiredParameterInformation> listInfoCommandRequiredParameters = new List<RequiredParameterInformation>() { requiredSource };

            List<ICommandLineCommand> commandLineCommands = new List<ICommandLineCommand>();
            CommandParameters createCommandParameters = new CommandParameters(@"C:\Users\Jensh\Desktop\Testdatein", @"test.jth", new RLECompressionAlgorithm());


            Action<CommandParameters> createAction = (parameter) =>
            {


            //TODO TODO TODO check the parameter properties , source and destination can not be null,compression can be
            CreateArchiveCommand createArchive = new CreateArchiveCommand(parameter.Source, parameter.Destination, parameter.CompressionAlgorithm);


            };

            Action<CommandParameters> appendAction = (parameter) =>
            {



            //the destination for the create command is only the foldername and file ending eg.: archive.dat, archive.jth
            //TODO TODO check the parameter properties source and destination can not be null compression can be
            ///////OR - IF THE REQUIRED PARAMETERS ARE THERE ARE ALREADY CHECKED IN THE PROCESS OUTSIDE; BUT THEY DONT WANT THAT NORMALLY.
            ArchiveAppendCommand appendArchive = new ArchiveAppendCommand(parameter.Source, parameter.Destination);


            };

            Action<CommandParameters> extractAction = (parameter) =>
            {



            //the destination for the create command is only the foldername and file ending eg.: archive.dat, archive.jth
            //TODO TODO check the parameter properties source and destination can not be null compression can be
            ExtractArchiveCommand extractArchive = new ExtractArchiveCommand(parameter.Source, parameter.Destination);


            };

            Action<CommandParameters> infoAction = (parameter) =>
            {



            //the destination for the create command is only the foldername and file ending eg.: archive.dat, archive.jth
            //TODO TODO check the parameter properties source and destination can not be null compression can be
            ArchiveInfoCommand archiveInfo = new ArchiveInfoCommand(parameter.Source);


            };

            Action<CommandParameters> listAction = (parameter) =>
            {



            //the destination for the create command is only the foldername and file ending eg.: archive.dat, archive.jth
            //TODO TODO check the parameter properties source and destination can not be null compression can be
            ListArchiveContentsCommand listArchiveContents = new ListArchiveContentsCommand(parameter.Source);


            };


            CommandLineProductiveCommand createCommand = new CommandLineProductiveCommand("-c", "--create", createAction, createOptionalParameters, createAppendExtractRequiredParameters);
            CommandLineProductiveCommand appendCommand = new CommandLineProductiveCommand("-a", "--append", appendAction, allOtherCommandsOptionalParameters, createAppendExtractRequiredParameters);
            CommandLineProductiveCommand extractCommand = new CommandLineProductiveCommand("-e", "--extract", extractAction, allOtherCommandsOptionalParameters, createAppendExtractRequiredParameters);
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
