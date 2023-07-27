//-----------------------------------------------------------------------
// <copyright file="RLECompressionParameter.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is used to specifiy wheter or not a given command should use RLE Compression. 
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    public class RLECompressionParameter : IParameter
    {
        private string shortParameterArgument;
        private string longParameterArgument;
        private Func<string[], bool> checkFunctionForParameterValidity;
        private object value;

        public RLECompressionParameter(string shortCommandName, string longCommandName)
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


        public string LongParameterName
        {
            get
            {
                return this.longParameterArgument;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.LongParameterName)} cannot be null!");
                }

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
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ShortParameterName)} cannot be null!");
                }

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
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.CheckParameterSpecificationForValidity)} cannot be null!");
                }

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
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.value)} can not be null!");
                }

                if (value != null && !(value is ICompressionAlgorithm))
                {
                    throw new ArgumentException($"{nameof(this.value)} must be an instace of {nameof(ICompressionAlgorithm)}!");
                }

                this.value = value;
            }
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

        public IParameter DeepCloneSelf()
        {
            return new RLECompressionParameter(this.ShortParameterName, this.LongParameterName);
        }
    }
}