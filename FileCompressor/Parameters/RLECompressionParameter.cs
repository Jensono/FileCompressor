using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    public class RLECompressionParameter : IParameter
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


        public RLECompressionParameter(string shortCommandName,string longCommandName)
        {
            this.LongParameterName = longCommandName;
            this.ShortParameterName = shortCommandName;
            this.CheckParameterSpecificationForValidity = (parameter) =>
            {
                if (parameter.Length == 0)
                {
                    return true;
                }
                return false;
            };
            this.ParseArgumentSpecificationAsValue = (parameter) =>
            {
                return null;
            };


        }
    }
}
