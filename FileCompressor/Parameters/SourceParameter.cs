using System;

namespace FileCompressor
{
    public class SourceParameter : IParameter
    {
        private string shortParameterArgument;
        private string longParameterArgument;
        private object value;
        private Func<string[], bool> checkFunctionForParameterValidity;


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
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.value)} can not be null!");
                }

                if (value != null && !(value is string))
                {
                    throw new ArgumentException($"{nameof(this.value)} must be a string!");
                }
                this.value = value;
            }
        }

        public SourceParameter(string shortCommandName, string longCommandName)
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

            
        }

        public bool TryParseValueAndSetIt(string[] array)
        {
            if (this.CheckParameterSpecificationForValidity(array))
            {
                if (array[0] != null)
                {
                    this.Value = array[0];

                }
               
                return true;
            }
            return false;
        }

        public IParameter DeepCloneSelf()
        {
            return new SourceParameter(this.ShortParameterName, this.LongParameterName);
        }
    }
}