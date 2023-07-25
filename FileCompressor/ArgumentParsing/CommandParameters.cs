

namespace FileCompressor
{
    using System;
    using System.Collections.Generic;

    public class CommandParameters
    {
        private List<IParameter> parameterList;
        private string commandShortName;

       

        // dont check these strings, if there false errorcodes will be thrown in the respective commands anyways, nobody can know if a given path is valid or not before executing a command , becouse paths can change during runtime.
        // THE COMPRESSIONALGO also doesnt need validation, it is null by defoult and only changes when the given command is create so leave it as is.
        public CommandParameters(List<IParameter> parameterList, string commandShortName)
        {
            this.ParameterList = parameterList;
            this.CommandShortName = commandShortName;
        }

        public List<IParameter> ParameterList
        {
            get
            {
                return this.parameterList;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ParameterList)} cannot be null!");
                }

                this.parameterList = value;
            }
        }

        public string CommandShortName
        {
            get
            {
                return this.commandShortName;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.CommandShortName)} cannot be null!");
                }

                this.commandShortName = value;
            }
        }

        public string TurnIntoCommandString()
        {
            string returnString = this.CommandShortName;

            // todo null check for the properties
            foreach (IParameter item in this.ParameterList)
            {
                returnString += $" {item.ShortParameterName} {item.Value}";
            }

            return returnString;
        }
    }
}