using System;

namespace FileCompressor
{
    public class OptionalParameterInformation : IParameter
    {
        private string shortParameterName { get; set; }
        private string longParameterName { get; set; }

        public string LongParameterName
        {
            get
            {
                return this.longParameterName;
            }
            set
            {
                this.longParameterName = value;
            }
        }

        public string ShortParameterName
        {
            get
            {
                return this.shortParameterName;
            }
            set
            {
                this.shortParameterName = value;
            }
        }
        public OptionalParameterInformation(string shortParameterName, string longParameterName, Func<string[], bool> checkForValidityFunction)
        {
            this.ShortParameterName = shortParameterName;
            this.LongParameterName = longParameterName;
            this.CheckParameterSpecificationForValidity = checkForValidityFunction;
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

        private Func<string[], bool> checkFunctionForParameterValidity;
    }
}