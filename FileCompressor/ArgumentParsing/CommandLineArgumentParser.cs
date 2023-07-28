//-----------------------------------------------------------------------
// <copyright file="CommandLineArgumentParser.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is used to parse user given commands into usable classes that contain the action needed to perform the command. It reads user input, analyses it and creates commands with the specified parameters based on the 
// given strings.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class is used to parse user given commands into usable classes that contain the action needed to perform the command. It reads user input, analyses it and creates commands with the specified parameters based on the 
    /// given strings. 
    /// </summary>
    public class CommandLineArgumentParser
    {
        /// <summary>
        /// The field for the string array of the arguments that need to be parsed.
        /// </summary>
        private string[] arguments;

        /// <summary>
        /// The field for the List of the <see cref="ICommandLineCommand"/> that are currently usable inside the application.
        /// </summary>
        private List<ICommandLineCommand> currentlyUsableCommands;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgumentParser"/> class. 
        /// </summary>
        /// <param name="usableCommands"> The commands that can currently be used inside of the application.</param>
        /// <param name="givenArguments"> The arguments provided by the user in the command line. </param>
        public CommandLineArgumentParser(List<ICommandLineCommand> usableCommands, string[] givenArguments)
        {
            this.CurrentlyUsableCommands = usableCommands;
            this.Arguments = givenArguments;
        }

        /// <summary>
        /// Gets or sets the string array of the arguments that need to be parsed.
        /// </summary>
        /// <value> Returns the list of arguments that should be parsed.</value>
        public string[] Arguments
        {
            get
            {
                return this.arguments;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.Arguments)} cannot be null!");
                }

                this.arguments = value;
            }
        }

        /// <summary>
        /// Gets or sets the List of the <see cref="ICommandLineCommand"/> that are currently usable inside the application.
        /// </summary>
        /// <value> A List of <see cref="ICommandLineCommand"/> that can be used inside the applications command line.</value>
        public List<ICommandLineCommand> CurrentlyUsableCommands
        {
            get
            {
                return this.currentlyUsableCommands;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.CurrentlyUsableCommands)} cannot be null!");
                }

                this.currentlyUsableCommands = value;
            }
        }

        /// <summary>
        /// This method tries to parse the given command line arguments, parse them and execute them one by one.
        /// </summary>
        /// <exception cref="ArchiveErrorCodeException"> 
        /// Will be thrown when: There were no arguments given except those that contain white spaces.
        ///                      A Archive error was thrown while parsing the arguments in a diffrent part of the application.
        ///                      If not all required parameters were given for all commands.
        ///                      If parameters were specified twice for the same command.                     
        /// </exception>
        public void ParseCommandsAndExecute()
        {
            // remove whitespaces at the end / trailing of the string[]
            string[] argumentsWithoutWhitespaces = this.RemoveWhiteSpaceEntry(this.Arguments);

            // if the only argument given was a whitespace command
            if (argumentsWithoutWhitespaces.Length == 0)
            {
                throw new ArchiveErrorCodeException("Errorcode 1. No Commands and Parameters were given!");
            }

            // turn all the commands and parameters to the small names
            // maybe this step is not needed but its easier to debug and also i never have to use the long names agai
            string[] smallNameCommands = this.TurnFoundCommandNamesIntoSmallNames(argumentsWithoutWhitespaces);
            string[] smallNameCommandsAndParameters = this.TurnFoundParameterNamesIntoSmallNames(smallNameCommands);

            // split the string array into smaller string arrays at every command
            // from  || -c || -s || [] || -d || [] || -rle || -a || -d || [] || -s || [] || ----to---> | -c | -s | [] | -d | [] |-rle || -a | -d | [] | -s | []  || .....
            List<string[]> commandStringArrays = new List<string[]>();
            try
            {
                commandStringArrays = this.SplitArgumentsByCommands(smallNameCommandsAndParameters);
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }

            // check if all required parameters are in the string[] ; also check to see if there are any dubilates, AT THIS POINT ANY COMMANDS AND PARAMETERS ARE small names
            bool areRequiredParametersThere;
            try
            {
                areRequiredParametersThere = this.CheckForRequiredParameters(commandStringArrays);
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }

            if (!areRequiredParametersThere)
            {
                // maybe remove the other errorcodes that are thrown, its being checked here anyway
                // i could also remove this exception and put it inside the CheckforRequiredParameter but then it would never return a bool and just throw Exception if false, which doesnt sound rigth.
                throw new ArchiveErrorCodeException("Errorcode 1. There are missing required parameters.");
            }

            // the commands were first split by their commands , now we furhter split them by the parameters, but still grouping commands with parameters logical units,
            // as the first entry in the list of a list is always the command by itself
            List<List<string[]>> commandsSplitIntoLogicalUnitsWithParametersSplit = this.SplitCommandListArrayFurtherIntoParametersLogicalUnits(commandStringArrays);

            // we create a list of commandparameters, commandparameters just hold a list of Iparameters and the commands short names that is associated with the parameters.
            try
            {
                List<CommandParameters> readParameteSpecification = this.CreateCommandParametersFromCommandListListArray(commandsSplitIntoLogicalUnitsWithParametersSplit);
                this.ExecuteCommands(readParameteSpecification);
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// This method removes all entries from a string array, that only consist of white spaces.
        /// </summary>
        /// <param name="array"> The array that should be changed to not containg any more entries, only consisting of white spaces.</param>
        /// <returns> A string array, that only contains entries that are not exclusibly made out of white spaces.</returns>
        private string[] RemoveWhiteSpaceEntry(string[] array)
        {
            return array.Where(arg => !string.IsNullOrWhiteSpace(arg)).ToArray();
        }

        /// <summary>
        /// This method executes all commands that were extracted from the command line, one after another as given by the user.
        /// </summary>
        /// <param name="readParameteSpecification"> A list with entries of <see cref="CommandParameters"/> which specify what command action with which parameters should be executed.</param>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown if a Archive Error was detected during execution of any of the commands.</exception>
        private void ExecuteCommands(List<CommandParameters> readParameteSpecification)
        {
            foreach (CommandParameters commandParameters in readParameteSpecification)
            {
                // maybe add an excpetion if there was not command found that is right, but could never happen here
                foreach (ICommandLineCommand availabeCommand in this.CurrentlyUsableCommands)
                {
                    if (commandParameters.CommandShortName.Equals(availabeCommand.ShortCommandName))
                    {
                        try
                        {
                            availabeCommand.ExecuteCommandAction(commandParameters.ParameterList);
                        }
                        catch (ArchiveErrorCodeException e)
                        {
                            e.AppendErrorCodeInformation($" Failed Execution. Failed to execute at Command String: {commandParameters.TurnIntoCommandString()}");
                            throw e;
                        }

                        // the right command was found and executed!
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This method creats a list of Command parameter objects from a list list array of string that were premodified, but initially given by the user.
        /// </summary>
        /// <param name="commandsListListArray"> A list of command blocks , which each contains a list of string arrays which are the logical units of either a command or a parameter with its specification.</param>
        /// <returns> A list of CommandParameters that could be extracted from the list of list of string arrays.</returns>
        /// <exception cref="ArchiveErrorCodeException"> Is raised when a parameter specification could not be read. For example -r 7. Here the parametetr specification for the retires parameter would be 7. </exception>
        /// <exception cref="InvalidOperationException"> Is raised when the entry in any of the above list, first entry in the second list, meaning the parameter specification could not be read or was invalid.</exception>
        private List<CommandParameters> CreateCommandParametersFromCommandListListArray(List<List<string[]>> commandsListListArray)
        {
            List<CommandParameters> foundCommandParameters = new List<CommandParameters>();

            // for every command
            foreach (List<string[]> item in commandsListListArray)
            {
                List<IParameter> parameterList = new List<IParameter>();

                // short command name is always the first entry in the string array in the first entry in the list
                string currentCommandShortName = item[0][0];

                // Extract the commandshort name that is in the beginning

                // for each part of one command that isnt the command itself
                for (int i = 1; i < item.Count; i++)
                {
                    IParameter currentParameter = this.BuildIParameterForListEntry(item[i]);
                    if (currentParameter == null)
                    {
                        throw new InvalidOperationException("There was a problem with the parameter, that should never occur");
                    }

                    // make a sub array if the original that does not contain the short name for the parameter but everything that follows it.
                    string[] parameterSpecification = new string[item[i].Length - 1];
                    Array.Copy(item[i], 1, parameterSpecification, 0, item[i].Length - 1);

                    // check to see if the parameter is parsable and parse it at the same time. If it is not parsable throw an errorcode
                    if (!currentParameter.TryParseValueAndSetIt(parameterSpecification))
                    {
                        // todo maybe specifiy the parameter and command where it happend for the user.
                        throw new ArchiveErrorCodeException($"Errorcode 1." +
                            $"Parameter Specifcation in Parameter: {currentParameter.ShortParameterName} " +
                            $"Specifaction: {this.ReverseStringArrayToString(parameterSpecification)} in Command: {currentCommandShortName} was invalid ");
                    }

                    parameterList.Add(currentParameter);
                }

                CommandParameters currentCommandParameters = new CommandParameters(parameterList, currentCommandShortName);
                foundCommandParameters.Add(currentCommandParameters);
            }

            return foundCommandParameters;
        }

        /// <summary>
        /// This method combines all entries of a string array and returns the resutlung string. Used primarily with string arrays from the command line .
        /// </summary>
        /// <param name="array"> The array of strings that should be combined to one string.</param>
        /// <returns> A string that combines each entry and seperates them with a space.</returns>
        private string ReverseStringArrayToString(string[] array)
        {
            string returnString = string.Empty;
            foreach (string entry in array)
            {
                returnString += entry + " ";
            }

            return returnString;
        }

        // before using this method we also validated that the given first entry is a available command so no checking

        /// <summary>
        /// This method build a <see cref="IParameter"/> object for a string array that contains a composit unit of a parameter calling and it specification. All of theses are their own entries in the string array.
        /// </summary>
        /// <param name="stringArray">The string array containg the parameters short name, in the first entry and specification parameters and their specification in all other entries.</param>
        /// <returns> A IParameter when the string array was a valid combination.</returns>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown when the string array was not a valid combination of parameters and parameter specifications. </exception>
        private IParameter BuildIParameterForListEntry(string[] stringArray)
        {
            List<IParameter> availableParamters = this.BuildAvailableParameterList(this.CurrentlyUsableCommands);
            foreach (IParameter item in availableParamters)
            {
                if (stringArray[0].Equals(item.ShortParameterName))
                {
                    // we need deep copys of the IParameter classes used, i think i will create a new method for IParameters called cloning to achieve this.
                    return item.DeepCloneSelf();
                }
            }

            throw new ArchiveErrorCodeException($"Errorcode 1. Given Parameter {stringArray[0]} was not a valid Parameter !");

            // read the first entry in the string array and find the corrseponding ParameterType from the List.
            // create a new object from that IParameter object, validate the given rest of the string and create the value for the parameter, then return the parameter
        }

        /// <summary>
        /// This method splits the initial command line arguments given, when the application is starded and splits them into logical units, for further processing.
        /// 
        /// </summary>
        /// <param name="commandStringArrays"> The list of string arrays which contain all the commands followed by their parameters and speciciations.</param>
        /// <returns> 
        /// Returns a List of List of String Arrays, inside the string arrays are string that were found between spaces when the user started the application, the first list layer groups commands together for example:
        /// -c -d [] -s [] ..... the list below that only contains parameters or the command itself so | -c | or |-d [] | and groups them together. These logical units can later be testes for validity by themself.
        /// </returns>
        private List<List<string[]>> SplitCommandListArrayFurtherIntoParametersLogicalUnits(List<string[]> commandStringArrays)
        {
            List<List<string[]>> returnListSplitByParameter = new List<List<string[]>>();

            // for every entry in the list
            for (int i = 0; i < commandStringArrays.Count; i++)
            {
                // for every string array, skipping the first entry as it is the command itself
                List<string[]> currentCommandListSplitAtParameters = new List<string[]>();

                string[] commandEntry = new string[] { commandStringArrays[i][0] };
                currentCommandListSplitAtParameters.Add(commandEntry);

                int currentParameterStartIndex = 1;

                for (int j = currentParameterStartIndex; j < commandStringArrays[i].Length; j++)
                {
                    // If the current string is a parameter and not the first string in its scope, split the array from the current commandStartIndex until this index
                    if (this.IsStringParameterShortName(commandStringArrays[i][j]) && j != 1)
                    {
                        string[] parameterGrouping = new string[j - currentParameterStartIndex];

                        // start copying the array from the commandstartindex of the smallnameCommands, for the length that was calculated in the commandGrouping
                        Array.Copy(commandStringArrays[i], currentParameterStartIndex, parameterGrouping, 0, parameterGrouping.Length);
                        currentCommandListSplitAtParameters.Add(parameterGrouping);
                        currentParameterStartIndex = j;
                    }
                }

                // if there are any commands left  at the end of the string array, i can handle it by just converting the end to a grouping too
                if (currentParameterStartIndex < commandStringArrays[i].Length)
                {
                    string[] lastParameterGrouping = new string[commandStringArrays[i].Length - currentParameterStartIndex];
                    Array.Copy(commandStringArrays[i], currentParameterStartIndex, lastParameterGrouping, 0, lastParameterGrouping.Length);
                    currentCommandListSplitAtParameters.Add(lastParameterGrouping);
                }

                returnListSplitByParameter.Add(currentCommandListSplitAtParameters);
            }

            return returnListSplitByParameter;
        }

        /// <summary>
        /// This method checks whether or not a string is the short parameter name for a parameter.
        /// </summary>
        /// <param name="userInput"> The string that should be checked.</param>
        /// <returns> Whether or not the string was a short parameter name.</returns>
        private bool IsStringParameterShortName(string userInput)
        {
            List<IParameter> currentParameters = this.BuildAvailableParameterList(this.CurrentlyUsableCommands);

            for (int i = 0; i < currentParameters.Count; i++)
            {
                if (userInput.Equals(currentParameters[i].ShortParameterName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method checks if all the required parameters were given for a command.
        /// </summary>
        /// <param name="commandStringArrays"> A list of string arrays, the first entry must containg a string array of length 1 which is the command short calling name itself.
        /// The following list entries are the logical parameter grouping, in these string arrays the first string must always be the parameters short name calling and the following entries in the array the specification for this particular parameter.
        /// </param>
        /// <returns> Whether or not the command had all its required parameters and if none of the parameters where specified twice. </returns>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown if: A parmater was specified twice.
        /// </exception>
        private bool CheckForRequiredParameters(List<string[]> commandStringArrays)
        {
            for (int i = 0; i < commandStringArrays.Count; i++)
            {
                // skip the first entry of each list becouse that should be a command anyway
                // first check for dublicates
                // find the correspondiong Command for the first entry
                ICommandLineCommand currentCommand = this.ReturnCorrespondingCommandFromShortName(commandStringArrays[i][0]);

                if (currentCommand == null)
                {
                    // command didnt exist
                    return false;
                }

                List<IParameter> requiredParameters = currentCommand.RequiredParamters;

                bool[] foundRequiredParameters = new bool[requiredParameters.Count];

                // create an array with the same number of reuiqred parameters and fill them with true if the parameter was found
                for (int j = 1; j < commandStringArrays[i].Length; j++)
                {
                    // go through all entries and find out if all the requireds are here, if a requiremend is found the bool array entry that represents that requirement parameter changes to true, if the bool array already is true
                    // an error is send, becouse the parameter was a dublicate
                    for (int k = 0; k < requiredParameters.Count; k++)
                    {
                        if (commandStringArrays[i][j].Equals(requiredParameters[k].ShortParameterName))
                        {
                            if (foundRequiredParameters[k] == true)
                            {
                                // the class that uses this needs to throw errorcodes
                                throw new ArchiveErrorCodeException($"Errorcode 1. Required Parameter {requiredParameters[k].LongParameterName} was specified twice!");
                            }
                            else
                            {
                                foundRequiredParameters[k] = true;
                            }
                        }
                    }
                }

                // if at the end of the loop the bool array isnt filled with true values than the requirements arent there. And the arguments are invalid
                if (!this.CheckBoolArrayForAllTrues(foundRequiredParameters))
                {
                    return false;
                }
            }

            // if no errorcode is reached then the requirements are inside the string array, could cahnge throwing an error for a boolarray that is as long as the string array list,
            // and fill that array with trues as one parses all the argumetns:
            return true;
        }

        /// <summary>
        /// This method checks whether or not a boolean array, only contains true values.
        /// </summary>
        /// <param name="foundRequiredParameters"> The array that should be checked.</param>
        /// <returns> Returns true if all entries of the array were true. Else false.</returns>
        private bool CheckBoolArrayForAllTrues(bool[] foundRequiredParameters)
        {
            for (int i = 0; i < foundRequiredParameters.Length; i++)
            {
                if (foundRequiredParameters[i] == false)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This method returns a coresponding <see cref="ICommandLineCommand"/> for a given command short name calling.
        /// </summary>
        /// <param name="commandShortName"> The string of the command short name for which, a command line command should be returned.</param>
        /// <returns> A command line command that is associated with the command short name. If no such command is found, null is returned. </returns>
        private ICommandLineCommand ReturnCorrespondingCommandFromShortName(string commandShortName)
        {
            foreach (var item in this.CurrentlyUsableCommands)
            {
                if (commandShortName.Equals(item.ShortCommandName))
                {
                    return item;
                }
            }

            // the only class that uses this method returns false if null is returned so its safe for now
            return null;
        }

        /// <summary>
        /// This method splits a argument by the command short name callings and makes seperate list entries at the split point.
        /// </summary>
        /// <param name="smallNameCommands"> The command line input that was given that should be split the short command names.</param>
        /// <returns> A List of string arrays, each entry in the list is a command that should be able to execute own its own with its own parameters and parameter specifications.</returns>
        /// <exception cref="ArchiveErrorCodeException"> Is thrown when the first entry in the given array is not a command short name, meaning the user first input was not a command name.</exception>
        private List<string[]> SplitArgumentsByCommands(string[] smallNameCommands)
        {
            List<string[]> commandsSplitInList = new List<string[]>();

            int currentCommandStartIndex = 0;

            if (!this.IsStringCommandShortName(smallNameCommands[0]))
            {
                throw new ArchiveErrorCodeException("Errorcode 1. The first argument given was no Command Short or Command Long sequence!");
            }

            for (int i = 0; i < smallNameCommands.Length; i++)
            {
                // If the current string is a command and not the first string in its scope split the array from the current commandStartIndex until this index
                if (this.IsStringCommandShortName(smallNameCommands[i]) && i != 0)
                {
                    string[] commandGrouping = new string[i - currentCommandStartIndex];

                    // start copying the array from the commandstartindex of the smallnameCommands, for the length that was calculated in the commandGrouping
                    Array.Copy(smallNameCommands, currentCommandStartIndex, commandGrouping, 0, commandGrouping.Length);
                    commandsSplitInList.Add(commandGrouping);
                    currentCommandStartIndex = i;
                }
            }

            // if there are any commands left  at the end of the string array, i can handle it by just converting the end to a grouping too
            if (currentCommandStartIndex < smallNameCommands.Length)
            {
                string[] commandGroup = new string[smallNameCommands.Length - currentCommandStartIndex];
                Array.Copy(smallNameCommands, currentCommandStartIndex, commandGroup, 0, commandGroup.Length);
                commandsSplitInList.Add(commandGroup);
            }

            return commandsSplitInList;
        }

        /// <summary>
        /// This method searches a string array for command long names and turns them into command short names.
        /// </summary>
        /// <param name="oldArguments"> The intial array which should be modified.</param>
        /// <returns> A string array which now contains command short names, where before there were entries with command long names.</returns>
        private string[] TurnFoundCommandNamesIntoSmallNames(string[] oldArguments)
        {
            string[] newArguments = new string[oldArguments.Length];

            for (int i = 0; i < oldArguments.Length; i++)
            {
                bool commandLongNameFound = false;

                // check all longnames, if it is the same use the small name
                for (int j = 0; j < this.CurrentlyUsableCommands.Count; j++)
                {
                    if (oldArguments[i].Equals(this.CurrentlyUsableCommands[j].LongCommandName))
                    {
                        newArguments[i] = this.CurrentlyUsableCommands[j].ShortCommandName;
                        commandLongNameFound = true;
                    }
                }

                // if string is not the long name of any commands then just add whatever is in the oldstring
                if (!commandLongNameFound)
                {
                    newArguments[i] = oldArguments[i];
                }
            }

            return newArguments;
        }

        /// <summary>
        /// This method  searches a string array for parameer long names and turns them into command short names.
        /// </summary>
        /// <param name="oldArguments">  The intial array which should be modified. </param>
        /// <returns> A string array which now contains parameter short names, where before there were entries with parameter long names.</returns>
        private string[] TurnFoundParameterNamesIntoSmallNames(string[] oldArguments)
        {
            string[] newArguments = new string[oldArguments.Length];

            // build a reportiar for parameter long and associated long names
            List<IParameter> availableParameters = this.BuildAvailableParameterList(this.CurrentlyUsableCommands);

            for (int i = 0; i < oldArguments.Length; i++)
            {
                bool commandLongNameFound = false;

                // check all longnames, if it is the same use the small name
                for (int j = 0; j < availableParameters.Count; j++)
                {
                    if (oldArguments[i].Equals(availableParameters[j].LongParameterName))
                    {
                        newArguments[i] = availableParameters[j].ShortParameterName;
                        commandLongNameFound = true;
                    }
                }

                // if string is not the long name of any commands then just add whatever is in the oldstring
                if (!commandLongNameFound)
                {
                    newArguments[i] = oldArguments[i];
                }
            }

            return newArguments;
        }

        /// <summary>
        /// This method build a list of all the parameters for which the command line arguments given by the user can be search for. It does this by using the currently usable commands which contain all the parameter information anyway.
        /// </summary>
        /// <param name="currentlyUsableCommands"> The list of commands that can currently be used whit this application.</param>
        /// <returns> A list of <see cref="IParameter"/> entries which represent every available parameter the user can use in the application.</returns>
        private List<IParameter> BuildAvailableParameterList(List<ICommandLineCommand> currentlyUsableCommands)
        {
            List<IParameter> returnListParameters = new List<IParameter>();

            // add all the optional and required parameter list
            foreach (ICommandLineCommand item in currentlyUsableCommands)
            {
                foreach (IParameter entry in item.RequiredParamters)
                {
                    if (!returnListParameters.Contains(entry))
                    {
                        returnListParameters.Add(entry);
                    }
                }

                foreach (IParameter entry in item.OptionalParameters)
                {
                    if (!returnListParameters.Contains(entry))
                    {
                        returnListParameters.Add(entry);
                    }
                }
            }

            return returnListParameters;
        }

        /// <summary>
        /// This method checks whether or not a string is the short command name calling of a command.
        /// </summary>
        /// <param name="stringToCheck"> The string to check.</param>
        /// <returns> A boolean value indicating whether or not the string was the short command name of a command.</returns>
        private bool IsStringCommandShortName(string stringToCheck)
        {
            for (int i = 0; i < this.CurrentlyUsableCommands.Count; i++)
            {
                if (stringToCheck.Equals(this.CurrentlyUsableCommands[i].ShortCommandName))
                {
                    return true;
                }
            }

            return false;
        }
    }
}