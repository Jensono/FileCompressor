using System;

namespace FileCompressor
{
    public class RetriesParameter : IParameter
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
        public RetriesParameter(string shortCommandName, string longCommandName)
        {
            this.LongParameterName = longCommandName;
            this.ShortParameterName = shortCommandName;
            this.CheckParameterSpecificationForValidity = (parameter) =>
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

            this.ParseArgumentSpecificationAsValue = (parameter) =>
            {
                if (parameter.Length >= 2)
                {
                    return 1;
                }
                // if there is no extra argument then its also valid, taking the value of 1
                if (parameter.Length == 0)
                {
                    return 1;
                }

                int potentialRepeatArgument;
                if (!int.TryParse(parameter[0], out potentialRepeatArgument))
                {
                    return 1;
                }

                if (potentialRepeatArgument < 0 || potentialRepeatArgument > 10)
                {
                    return 1;
                }
                //only lands here if the lenght of the array is one and the string is a integer between
                return potentialRepeatArgument;
            };
        }
    }
}