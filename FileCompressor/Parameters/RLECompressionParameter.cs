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
            { //mabye add checks for this but not sure; WHY CANT I SET THE Interface to only allow private sets smh
                this.value = value;
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
            


        }

        public bool TryParseValueAndSetIt(string[] array)
        {
            if (this.CheckParameterSpecificationForValidity(array))
            {
                this.Value = new RLECompressionAlgorithm();
                return true;
            }
            return false;
        }
    }
}
