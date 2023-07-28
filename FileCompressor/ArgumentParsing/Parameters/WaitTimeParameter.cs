//-----------------------------------------------------------------------
// <copyright file="WaitTimeParameter.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class can save values for the the wait time parameters that is used inside some commands. It speficies how much time should pass before a given command should be repeated, after it failed to execute the first time.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;

    /// <summary>
    /// This class can save values for the the wait time parameters that is used inside some commands. It specifies  how much time should pass before a given command should be repeated, after it failed to execute the first time.
    /// </summary>
    public class WaitTimeParameter : IParameter
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
        /// Initializes a new instance of the <see cref="WaitTimeParameter"/> class. 
        /// </summary>
        /// <param name="shortParameterName"> The short Parameter name for the wait time parameter.</param>
        /// <param name="longParameterName"> The long Parameter name for the wait time parameter.</param>
        public WaitTimeParameter(string shortParameterName, string longParameterName)
        {
            this.LongParameterName = longParameterName;
            this.ShortParameterName = shortParameterName;
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

        /// <summary>
        /// This method tries to parse the value for the wait time parameters.
        /// </summary>
        /// <param name="array"> The arguments that should be parsed for the parameter specifcation.</param>
        /// <returns> Returns true if the string array was parsable as the parameters specfications. </returns>
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

        /// <summary>
        /// This method makes a deep copy of the wait time parameter itself.
        /// </summary>
        /// <returns> Returns a IParameter that is also a wait time parameter.</returns>
        public IParameter DeepCloneSelf()
        {
            return new WaitTimeParameter(this.ShortParameterName, this.LongParameterName);
        }
    }
}