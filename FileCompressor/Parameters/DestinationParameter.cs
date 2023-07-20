using System;

namespace FileCompressor
{
    public class DestinationParameter : IParameter
    {

        private string shortParameterArgument;
        private string longParameterArgument;
        private Func<string[], bool> checkFunctionForParameterValidity;

        private Func<string[], object> parseArgumentSpecificationAsValue;

        public string LongParameterName
        {
            get
            {
                return this.longParameterArgument;
            }
            set
            {
                this.longParameterArgument = value;
            }
        }

        public string ShortParameterName
        {
            get
            {
                return this.shortParameterArgument;
            }
            set
            {
                this.shortParameterArgument = value;
            }
        }

        public Func<string[], bool> CheckParameterSpecificationForValidity
        {
            get
            {
                return this.checkFunctionForParameterValidity;
            }
            set
            {
                this.checkFunctionForParameterValidity = value;
            }
        }

        public Func<string[], object> ParseArgumentSpecificationAsValue
        {
            get
            {
                return this.parseArgumentSpecificationAsValue;
            }
            set
            {
                this.parseArgumentSpecificationAsValue = value;
            }
        }

        public DestinationParameter(string shortCommandName, string longCommandName)
        {
            this.LongParameterName = longCommandName;
            this.ShortParameterName = shortCommandName;
            this.CheckParameterSpecificationForValidity = (parameter) =>
            {

                if (parameter.Length >= 2)
                {
                    return false;
                }

                if (parameter.Length == 0)
                {
                    return false;
                }

                //no additional validation can be done becouse souce and destination can be very diffrent things depending on the command,
                //they can be filenames, directories, i could check if it is a valid path or not but the commands do that anyway.
                //also commands can have paths in them that will only be created after the command entered runtime which makes
                //it impossible to confirm if a path is valid or not before excectuion and runtime.
                return true;

            };

            this.ParseArgumentSpecificationAsValue = (parameter) =>
            {
                return parameter[0];
            };
        }

    }
}