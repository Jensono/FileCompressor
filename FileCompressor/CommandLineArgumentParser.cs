using System;
using System.Collections.Generic;

namespace FileCompressor
{
    public class CommandLineArgumentParser
    {
        private string[] arguments;
        private List<ICommandLineCommand> currentlyUsableCommands;

        //todo exceptions for properties

        public string[] Arguments
        {
            get
            {
                return this.arguments;
            }
            set
            {
                this.arguments = value;
            }
        }

        public List<ICommandLineCommand> CurrentlyUsableCommands
        {
            get
            {
                return this.currentlyUsableCommands;
            }
            set
            {
                this.currentlyUsableCommands = value;
            }
        }

        public CommandLineArgumentParser(List<ICommandLineCommand> usableCommands, string[] givenArguments)
        {
            this.CurrentlyUsableCommands = usableCommands;
            this.Arguments = givenArguments;
        }

        // create a new class that can take a string[] array and that then scans for commands, first all longcommandnames will be changed to short command names
        // after that the list is seperated into diffrent string[], for all ARRAYS the corresponding list of required parameters is taken as a foundation for scanning the now smaller string[].
        // If all required parameters are found now scan for optional parameters

        //MAYBE ALREADY SPLIT THIS INTO Parse and then execute
        public void ParseCommandsAndExecute()
        {
            //remove whitespaces at the end / trailing of the string[] 
           string[] argumentsWithoutEndWhitespaces = this.RemoveLastWhiteSpaceEntry(this.Arguments);
            //turn all the commands and parameters to the small names
            // maybe this step is not needed but its easier to debug and also i never have to use the long names agai
            string[] smallNameCommands = this.TurnFoundCommandNamesIntoSmallNames(argumentsWithoutEndWhitespaces);
            string[] smallNameCommandsAndParameters = this.TurnFoundParameterNamesIntoSmallNames(smallNameCommands);
            //split the string array into smaller string arrays at every command
            //from  || -c || -s || [] || -d || [] || -rle || -a || -d || [] || -s || [] || ----to---> | -c | -s | [] | -d | [] |-rle || -a | -d | [] | -s | []  || .....
            List<string[]> commandStringArrays = this.SplitArgumentsByCommands(smallNameCommands);
            //check if all required parameters are in the string[] ; also check to see if there are any dubilates, AT THIS POINT ANY COMMANDS AND PARAMETERS ARE small names
            bool areRequiredParametersThere = this.CheckForRequiredParameters(commandStringArrays);

            if (!areRequiredParametersThere)
            {
                //maybe remove the other errorcodes that are thrown, its being checked here anyway
                throw new ArchiveErrorCodeException("ArchiveErrorcode 1, arguments could not be parsed!");
            }
            //the commands were first split by their commands , now we furhter split them by the parameters, but still grouping commands with parameters logical units,
            //as the first entry in the list of a list is always the command by itself

            List<List<string[]>> commandsSplitIntoLogicalUnitsWithParametersSplit = this.SplitCommandListArrayFurtherIntoParametersLogicalUnits(commandStringArrays);
            // we create a list of commandparameters, commandparameters just hold a list of Iparameters and the commands short names that is associated with the parameters.

            List<CommandParameters> readParameteSpecification = this.CreateCommandParametersFromCommandListListArray(commandsSplitIntoLogicalUnitsWithParametersSplit);

            this.ExecuteCommands(readParameteSpecification);
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////// READ THIS TOMORrOW //////////////////////////////////////////////////////////////////////////////
            /////now we actually process the list of lists, nnde build a list of IParameters, which can be send to the CommandExecuterclass. That class generates a ICommand object and from the parameterlist and excecutes it. it does this in a
            //for loop outisde which contains the wait and the retries parameter values, if there were given any

            // we need to link the iparameter lits to the same entry as the command itself, then try catch the execution for all entries, if a errorcode is thrown throw it up and send it to the console, as this is the last class used
            // in main

            //we look what parameterinformation matches the entries (starting at the second entry = 1 ) of the second layer list and look at the first element in the string array. This must now contain the small name parameter argument.
            //we split the rest of the parameter arguments and send them to the Function that validates a parameters arguments.If any of the parameter specifiers are wrong we send Errorcode 1.
            //if the parameter was validated we can put it in the commandparameters,or we could do that after the whole second layer list was validated.

            //Check if there are any strings left that arent needed flase-->errorcode
            //false--> errorcode

            //generate a commandParameter object from all the parameters that were found
            // connect the commands with the commandparameters and invoke them one by one
        }

        private string[] RemoveLastWhiteSpaceEntry(string[] arguments)
        {
            //if it does contains only whitespaces at the last arguments just remove that entry
            if (string.IsNullOrWhiteSpace(arguments[arguments.Length-1]))
            {
                string[] argumentArrayWithoutLastEntry = new string[arguments.Length - 1];
                Array.Copy(arguments, argumentArrayWithoutLastEntry, arguments.Length - 1);
                return argumentArrayWithoutLastEntry;
            }
            //else just return the original arguments array
            else
            {
                return arguments;
            }
        }

        private void ExecuteCommands(List<CommandParameters> readParameteSpecification)
        {
            foreach (CommandParameters commandParameters in readParameteSpecification)
            {
                //maybe add an excpetion if there was not command found that is right, but could never happen here
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
                            //todo errorcode 
                            e.AppendErrorCodeInformation($"Failed to execute Command String: {commandParameters.TurnIntoCommandString()}");
                            throw e;
                        }
                        
                        //the right command was found and executed!
                        break;
                    }
                }
            }
        }

        private List<CommandParameters> CreateCommandParametersFromCommandListListArray(List<List<string[]>> commandsListListArray)
        {
            List<CommandParameters> foundCommandParameters = new List<CommandParameters>();
            //for every command
            foreach (List<string[]> item in commandsListListArray)
            {
                List<IParameter> parameterList = new List<IParameter>();

                // short command name is always the first entry in the string array in the first entry in the list
                string currentCommandShortName = item[0][0];
                //Extract the commandshort name that is in the beginning

                //for each part of one command that isnt the command itself
                for (int i = 1; i < item.Count; i++)
                {
                    IParameter currentParameter = this.BuildIParameterForListEntry(item[i]);
                    if (currentParameter == null)
                    {
                        throw new InvalidOperationException("There was a problem with the parameter, that should never occur");
                    }

                    //make a sub array if the original that does not contain the short name for the parameter but everything that follows it.
                    string[] parameterSpecification = new string[item[i].Length - 1];
                    Array.Copy(item[i], 1, parameterSpecification, 0, item[i].Length - 1);

                    //check to see if the parameter is parsable and parse it at the same time. If it is not parsable throw an errorcode
                    if (!currentParameter.TryParseValueAndSetIt(parameterSpecification))
                    {
                        //maybe specifiy the parameter and command where it happend for the user.
                        throw new ArchiveErrorCodeException("error and shit todo");
                    }

                    parameterList.Add(currentParameter);
                }

                CommandParameters currentCommandParameters = new CommandParameters(parameterList, currentCommandShortName);
                foundCommandParameters.Add(currentCommandParameters);
            }
            return foundCommandParameters;
        }

        //before using this method we also validated that the given first entry is a available command so no checking
        private IParameter BuildIParameterForListEntry(string[] stringArray)
        {
            List<IParameter> availableParamters = this.BuildAvailableParameterList(this.CurrentlyUsableCommands);
            foreach (IParameter item in availableParamters)
            {
                if (stringArray[0].Equals(item.ShortParameterName))
                {
                    //we need deep copys of the IParameter classes used, i think i will create a new method for IParameters called cloning to achieve this.
                    return item.DeepCloneSelf();
                }
            }

            throw new ArchiveErrorCodeException("no such command found, todo");
            //read the first entry in the string array and find the corrseponding ParameterType from the List.
            //create a new object from that IParameter object, validate the given rest of the string and create the value for the parameter, then return the parameter
        }

        /// <summary>
        /// Returns a List of List of String Arrays, inside the string arrays are string that were found between spaces when the user started the application, the first list layer groups commands toegther for example:
        /// -c -d [] -s [] ..... the list belowe that only contains parameters or the command itself so |-c | or |-d []| and groups them together. These logical units can later be testes for validity by themself.
        /// </summary>
        /// <param name="commandStringArrays"></param>
        /// <returns></returns>
        private List<List<string[]>> SplitCommandListArrayFurtherIntoParametersLogicalUnits(List<string[]> commandStringArrays)
        {
            List<List<string[]>> returnListSplitByParameter = new List<List<string[]>>();

            //for every entry in the list
            for (int i = 0; i < commandStringArrays.Count; i++)
            {
                //for every string array, skipping the first entry as it is the command itself
                List<string[]> currentCommandListSplitAtParameters = new List<string[]>();

                string[] commandEntry = new string[] { commandStringArrays[i][0]};
                currentCommandListSplitAtParameters.Add(commandEntry);

                int currentParameterStartIndex = 1;

                for (int j = currentParameterStartIndex; j < commandStringArrays[i].Length; j++)
                {
                    //If the current string is a parameter and not the first string in its scope, split the array from the current commandStartIndex until this index

                    if (this.IsStringParameterShortName(commandStringArrays[i][j]) && j != 1)
                    {
                        string[] parameterGrouping = new string[j - currentParameterStartIndex];
                        //start copying the array from the commandstartindex of the smallnameCommands, for the length that was calculated in the commandGrouping
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

        private bool CheckForRequiredParameters(List<string[]> commandStringArrays)
        {
            for (int i = 0; i < commandStringArrays.Count; i++)
            {
                //skip the first entry of each list becouse that should be a command anyway
                //first check for dublicates
                //find the correspondiong Command for the first entry

                ICommandLineCommand currentCommand = this.ReturnCorrespondingCommandFromShortName(commandStringArrays[i][0]);

                if (currentCommand == null)
                {
                    return false;
                    throw new ArgumentException("ERRORCODE 1 todo here");
                }

                List<IParameter> requiredParameters = currentCommand.RequiredParamters;

                bool[] foundRequiredParameters = new bool[requiredParameters.Count];
                //create an array with the same number of reuiqred parameters and fill them with true if the parameter was found
                for (int j = 1; j < commandStringArrays[i].Length; j++)
                {
                    //go through all entries and find out if all the requireds are here, if a requiremend is found the bool array entry that represents that requirement parameter changes to true, if the bool array already is true
                    //an error is send, becouse the parameter was a dublicate

                    for (int k = 0; k < requiredParameters.Count; k++)
                    {
                        if (commandStringArrays[i][j].Equals(requiredParameters[k].ShortParameterName))
                        {
                            if (foundRequiredParameters[k] == true)
                            {
                                //the class that uses this needs to throw errorcodes
                                return false;
                                throw new ArgumentException("ERRORCODE 1 todo here");
                            }
                            else
                            {
                                foundRequiredParameters[k] = true;
                            }
                        }
                    }
                }

                //if at the end of the loop the bool array isnt filled with true values than the requirements arent there. And the arguments are invalid
                if (!this.CheckBoolArrayForAllTrues(foundRequiredParameters))
                {
                    //maybe specifiy in the message why it didnt work todo
                    return false;
                    throw new ArgumentException("ERRORCODE 1 todo here");
                }
            }
            //if no errorcode is reached then the requirements are inside the string array, could cahnge throwing an error for a boolarray that is as long as the string array list,
            //and fill that array with trues as one parses all the argumetns:

            return true;
        }

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

        private ICommandLineCommand ReturnCorrespondingCommandFromShortName(string commandShortName)
        {
            foreach (var item in this.CurrentlyUsableCommands)
            {
                if (commandShortName.Equals(item.ShortCommandName))
                {
                    return item;
                }
            }
            return null;
        }

        private List<string[]> SplitArgumentsByCommands(string[] smallNameCommands)
        {
            List<string[]> commandsSplitInList = new List<string[]>();

            int currentCommandStartIndex = 0;

            //TODO
            //first string MUST BE A COMMAND or else the commandline is invalid by defoult and will not execute even one, THROW SOME SHIT
            if (!this.IsStringCommandShortName(smallNameCommands[0]))
            {
                throw new ArgumentException("Errocode 1 and shit todo");
            }

            for (int i = 0; i < smallNameCommands.Length; i++)
            {  
                //If the current string is a command and not the first string in its scope split the array from the current commandStartIndex until this index

                if (this.IsStringCommandShortName(smallNameCommands[i]) && i != 0)
                {
                    string[] commandGrouping = new string[i - currentCommandStartIndex];
                    //start copying the array from the commandstartindex of the smallnameCommands, for the length that was calculated in the commandGrouping
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

        private string[] TurnFoundCommandNamesIntoSmallNames(string[] oldArguments)
        {
            string[] newArguments = new string[oldArguments.Length];

            for (int i = 0; i < oldArguments.Length; i++)
            {
                bool commandLongNameFound = false;
                //check all longnames, if it is the same use the small name
                for (int j = 0; j < this.CurrentlyUsableCommands.Count; j++)
                {
                    if (oldArguments[i].Equals(this.CurrentlyUsableCommands[j].LongCommandName))
                    {
                        newArguments[i] = this.CurrentlyUsableCommands[j].ShortCommandName;
                        commandLongNameFound = true;
                    }
                }
                //if string is not the long name of any commands then just add whatever is in the oldstring
                if (!commandLongNameFound)
                {
                    newArguments[i] = oldArguments[i];
                }
            }

            return newArguments;
        }

        private string[] TurnFoundParameterNamesIntoSmallNames(string[] oldArguments)
        {
            string[] newArguments = new string[oldArguments.Length];

            //build a reportiar for parameter long and associated long names

            List<IParameter> availableParameters = this.BuildAvailableParameterList(this.CurrentlyUsableCommands);

            for (int i = 0; i < oldArguments.Length; i++)
            {
                bool commandLongNameFound = false;
                //check all longnames, if it is the same use the small name
                for (int j = 0; j < availableParameters.Count; j++)
                {
                    if (oldArguments[i].Equals(availableParameters[j].LongParameterName))
                    {
                        newArguments[i] = availableParameters[j].ShortParameterName;
                        commandLongNameFound = true;
                    }
                }
                //if string is not the long name of any commands then just add whatever is in the oldstring
                if (!commandLongNameFound)
                {
                    newArguments[i] = oldArguments[i];
                }
            }

            return newArguments;
        }

        private List<IParameter> BuildAvailableParameterList(List<ICommandLineCommand> currentlyUsableCommands)
        {
            List<IParameter> returnListParameters = new List<IParameter>();
            //add all the optional and required parameter list
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