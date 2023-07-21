using System;

namespace FileCompressor
{
    public class WaitTimeParameter : IParameter
    {
        private string shortParameterArgument;
        private string longParameterArgument;
        private object value;
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

      
        public object Value
        {
            get
            {
                return this.value;
            }
            set
            { //todo there MUST be checks to validate that a new values is either null or the object that resides in these classes - string, int etc.
                this.value = value;
            }
        }

        public WaitTimeParameter(string shortCommandName, string longCommandName)
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

           
        }

        public bool TryParseValueAndSetIt(string[] array)
        {
            if (!this.CheckParameterSpecificationForValidity(array))
            {
                return false;
            }

            if (array.Length == 0)
            {
                this.Value = 1;
                return true;
            }
            else 
            {
                this.Value = int.Parse(array[0]);
                return true;
            }
        }
    }
}