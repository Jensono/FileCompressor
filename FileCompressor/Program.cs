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
        {//////////////////////////////////////////////////////////////////////////////////////////////////////MOVE THIS SHIT NOW /////////////////////////////////////////////////////////////////////////////////////////////////
            //initialize the optional parameters //TODO DO THIS SOMEWHERE ELSE PLEASE, even fixedVariables is better than in main


            //RLE doesnt require any other arguments after it.
            Func<string, bool> checkForRLECopmressionCommandValidity =  (parameter)=> { return true; };

            Func<string, bool> checkForWaitCommandValidity = (parameter) => 

            {
                int potentialRepeatArgument;
                if (int.TryParse(parameter,out potentialRepeatArgument)
                {
                    if ( potentialRepeatArgument>0 && potentialRepeatArgument <= 10)
                    {
                        return true;
                    }

                }

                            };


            OptionalParameterInformation optionalCompressionType = new OptionalParameterInformation("-rle","--rleCompress",);
            OptionalParameterInformation optionalWaitTimeBetweenTriesType = new OptionalParameterInformation("-w", "--wait");
            OptionalParameterInformation optionalRetriesForCommandType = new OptionalParameterInformation("-r", "--retry");


            RequiredParameterInformation requiredSource = new RequiredParameterInformation("-s", "--source");
            RequiredParameterInformation requiredDestination = new RequiredParameterInformation("-d", "--destination");


            //create all optional List for the Command
            List<OptionalParameterInformation> createOptionalParameters = new List<OptionalParameterInformation>() { optionalCompressionType, optionalRetriesForCommandType, optionalWaitTimeBetweenTriesType };
            List<OptionalParameterInformation> allOtherCommandsOptionalParameters = new List<OptionalParameterInformation>() { optionalRetriesForCommandType, optionalWaitTimeBetweenTriesType };
            //CREATE ALL THE REQUIRED PARAMETER LIST FOR THE COMMANDS

            List<RequiredParameterInformation> createAppendExtractRequiredParameters = new List<RequiredParameterInformation>() { requiredDestination, requiredSource };
            List<RequiredParameterInformation> listInfoCommandRequiredParameters = new List<RequiredParameterInformation>() { requiredSource };

            List<ICommandLineCommand> commandLineCommands = new List<ICommandLineCommand>();
            CommandParameters createCommandParameters = new CommandParameters(@"C:\Users\Jensh\Desktop\Testdatein", @"test.jth", new RLECompressionAlgorithm());


            Action<CommandParameters> createAction = (parameter)=>
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


            CommandLineProductiveCommand createCommand = new CommandLineProductiveCommand("-c", "--create", createAction ,createOptionalParameters,createAppendExtractRequiredParameters);
            CommandLineProductiveCommand appendCommand = new CommandLineProductiveCommand("-a", "--append", appendAction,allOtherCommandsOptionalParameters,createAppendExtractRequiredParameters);
            CommandLineProductiveCommand extractCommand = new CommandLineProductiveCommand("-e","--extract", extractAction, allOtherCommandsOptionalParameters, createAppendExtractRequiredParameters);
            CommandLineProductiveCommand infoCommand= new CommandLineProductiveCommand("-i", "--info", infoAction, allOtherCommandsOptionalParameters, listInfoCommandRequiredParameters);
            CommandLineProductiveCommand listCommand = new CommandLineProductiveCommand("-l", "--list", listAction, allOtherCommandsOptionalParameters, listInfoCommandRequiredParameters);


            //make a list for all the commands and make it so its returnable for outside, create a new class that can take a string[] array and that then scans for commands, first all longcommandnames will be changed to short command names
            // after that the list is seperated into diffrent string[], for all ARRAYS the corresponding list of required parameters is taken as a foundation for scanning the now smaller string[]. If all required parameters are found now scan for 


            //////////////////////////////////////////////////////////////////////////////////////////////////////MOVE THIS SHIT NOW /////////////////////////////////////////////////////////////////////////////////////////////////




            //testing the final CommandLineProductiveCommand
                        
            //CreateArchiveCommand createArchiveCommand = new CreateArchiveCommand(@"C:\Users\Jensh\Desktop\Testdatein", "test.jth", new NoCompressionAlgorithm());

            //CreateArchiveCommand createArchiveCommand = new CreateArchiveCommand(@"C:\Users\Jensh\Desktop\Testdatein", "test.jth", new RLECompressionAlgorithm()) ;

            ArchiveInfoCommand archiveInfoCommand = new ArchiveInfoCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth");

            ListArchiveContentsCommand listArchiveContentsCommand = new ListArchiveContentsCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth");

            ExtractArchiveCommand extract = new ExtractArchiveCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth", @"C:\Users\Jensh\Desktop\new");

            ArchiveAppendCommand appendCommandFirstTry = new ArchiveAppendCommand(@"C:\Users\Jensh\Desktop\Testdatein_Extra", @"C:\Users\Jensh\Desktop\Testdatein\test.jth");

            ArchiveInfoCommand archiveInfoCommand2 = new ArchiveInfoCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth");

            ListArchiveContentsCommand listArchiveContentsCommand2 = new ListArchiveContentsCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth");

            ExtractArchiveCommand extract2 = new ExtractArchiveCommand(@"C:\Users\Jensh\Desktop\Testdatein\test.jth", @"C:\Users\Jensh\Desktop\appendoutput");

        
            



            //var test = new ArchiveHeader(484,true,1278127318);
            //byte[] archiveBytes = test.GetArchiveHeaderAsBytes();
            //var test2 = new ArchiveHeader(archiveBytes);
        }
    }
}
