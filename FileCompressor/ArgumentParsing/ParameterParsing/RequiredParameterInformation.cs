using System;

namespace FileCompressor
{
    public class RequiredParameterInformation : IParameter
    {
        private string shortParameterArgument { get; set; }
        private string longParameterArgument { get; set; }

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

        public Func<string[], bool> CheckParameterSpecificationForValidity {
            get
            {
                return this.checkFunctionForParameterValidity;
            }
            set
            {
                this.checkFunctionForParameterValidity = value;
            }
        }

        private Func<string[], bool> checkFunctionForParameterValidity;
        public RequiredParameterInformation(string shortParameterName, string longParameterName, Func<string[],bool> checkForValidityFunction)
        {
         this.CheckParameterSpecificationForValidity = checkForValidityFunction;
            this.ShortParameterName = shortParameterName;
            this.LongParameterName = longParameterName;
        }
    }
}