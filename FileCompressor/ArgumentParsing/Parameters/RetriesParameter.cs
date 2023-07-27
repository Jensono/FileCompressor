//-----------------------------------------------------------------------
// <copyright file="RetriesParameter.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class can save values for the the retries parameters that is used inside some commands. It speficies how many times a given command should be repeat , (and fail to execute) until
// the command was deemed "failed".
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;

    /// <summary>
    /// This class can save values for the the retries parameters that is used inside some commands. It specifies  how many times a given command should be repeat , (and fail to execute) until
    /// the command was deemed "failed".
    /// </summary>
    public class RetriesParameter : IParameter
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

                // only lands here if the lenght of the array is one and the string is a integer between
                return true;
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
        /// Gets or sets the value the parameter holds. Must be an integer.
        /// </summary>
        /// <value> The value of the parameter. Must be an integer. </value>
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

                if (value != null && !(value is int))
                {
                    throw new ArgumentException($"{nameof(this.value)} must be an integer!");
                }

                this.value = value;
            }
        }

        public bool TryParseValueAndSetIt(string[] argumentArray)
        {
            if (!this.CheckParameterSpecificationForValidity(argumentArray))
            {
                return false;
            }

            if (argumentArray.Length == 0)
            {
                this.Value = 1;
                return true;
            }
            else
            {   // must be a string array with one entry and parseable as an int
                this.Value = int.Parse(argumentArray[0]);
                return true;
            }
        }

        public IParameter DeepCloneSelf()
        {
            return new RetriesParameter(this.ShortParameterName, this.LongParameterName);
        }
    }
}