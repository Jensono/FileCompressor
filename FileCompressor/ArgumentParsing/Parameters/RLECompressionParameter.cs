﻿//-----------------------------------------------------------------------
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

    /// <summary>
    /// This class is used to specify whether or not a given command should use RLE Compression. 
    /// </summary>
    public class RLECompressionParameter : IParameter
    {
        /// <summary>
        ///  The field for the short name or calling of the parameter, that is also used by the user in the command line.
        /// </summary>
        private string shortParameterArgument;

        /// <summary>
        /// The field for the long name or calling of the parameter, that is also used by the user in the command line.
        /// </summary>
        private string longParameterArgument;

        /// <summary>
        /// The field for the Function that checks whether or not a parameter specification is valid.
        /// </summary>
        private Func<string[], bool> checkFunctionForParameterValidity;

        /// <summary>
        /// The field for the value the parameter holds.
        /// </summary>
        private object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RLECompressionParameter"/> class. 
        /// </summary>
        /// <param name="shortParameterName"> The short Parameter name for the RLE compression parameter.</param>
        /// <param name="longParameterName"> The long Parameter name for the RLE compression parameter.</param>
        public RLECompressionParameter(string shortParameterName, string longParameterName)
        {
            this.LongParameterName = longParameterName;
            this.ShortParameterName = shortParameterName;
            this.CheckParameterSpecificationForValidity = (parameter) =>
            {
                if (parameter.Length == 0)
                {
                    return true;
                }

                return false;
            };
        }

        /// <summary>
        /// Gets or sets the long name or calling of the parameter, that is also used by the user in the command line.
        /// </summary>
        /// <value> The long name or calling for the parameter. </value>
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

        /// <summary>
        /// Gets or sets the short name or calling of the parameter, that is also used by the user in the command line.
        /// </summary>
        /// <value> The short name or calling for the parameter. </value>
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

        /// <summary>
        /// Gets or sets the Function that checks whether or not a parameter specification is valid.
        /// </summary>
        /// <value> The Function that checks if the parameter specification are fulfilled. </value>
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

        /// <summary>
        /// Gets or sets the value the parameter holds. Must be an object of the <see cref="ICompressionAlgorithm"/> interface.
        /// </summary>
        /// <value> The value of the parameter. Must be an object of the <see cref="ICompressionAlgorithm"/> interface. </value>
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

        /// <summary>
        /// This method tries to parse the value for the RLE Compression parameters.
        /// </summary>
        /// <param name="array"> The arguments that should be parsed for the parameter specification.</param>
        /// <returns> Returns true if the string array could be parsed as the parameters specification. </returns>
        public bool TryParseValueAndSetIt(string[] array)
        {
            if (this.CheckParameterSpecificationForValidity(array))
            {
                this.Value = new RLECompressionAlgorithm();
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method makes a deep copy of the RLE Compression parameter itself.
        /// </summary>
        /// <returns> Returns a IParameter that is also a RLE Compression parameter.</returns>
        public IParameter DeepCloneSelf()
        {
            return new RLECompressionParameter(this.ShortParameterName, this.LongParameterName);
        }
    }
}