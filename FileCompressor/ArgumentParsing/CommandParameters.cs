//-----------------------------------------------------------------------
// <copyright file="CommandParameters.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is used to save the parameters used for command execution. It contains the command itself and the given parameters.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class is used to save the parameters used for command execution. It contains the command itself and the given parameters.
    /// </summary>
    public class CommandParameters
    {
        /// <summary>
        /// The field for the List of <see cref="IParameter"/> that is used for the Command.
        /// </summary>
        private List<IParameter> parameterList;

        /// <summary>
        /// The field for the associated commands short calling.
        /// </summary>
        private string commandShortName;



        // dont check these strings, if there false errorcodes will be thrown in the respective commands anyways, nobody can know if a given path is valid or not before executing a command , becouse paths can change during runtime.
        // THE COMPRESSIONALGO also doesnt need validation, it is null by defoult and only changes when the given command is create so leave it as is.

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandParameters"/> class. 
        /// </summary>
        /// <param name="parameterList"> The parameters for a specific command.</param>
        /// <param name="commandShortName"> The short name for the command, usually starts with '-'.</param>
        public CommandParameters(List<IParameter> parameterList, string commandShortName)
        {
            this.ParameterList = parameterList;
            this.CommandShortName = commandShortName;
        }

        /// <summary>
        /// Gets or sets the List of <see cref="IParameter"/> that is used for the Command.
        /// </summary>
        /// <value> The List of <see cref="IParameter"/> that is used for the Command.</value>
        public List<IParameter> ParameterList
        {
            get
            {
                return this.parameterList;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ParameterList)} cannot be null!");
                }

                this.parameterList = value;
            }
        }

        /// <summary>
        /// Gets or sets the associated commands short calling.
        /// </summary>
        /// <value> The associated commands short calling. </value>
        public string CommandShortName
        {
            get
            {
                return this.commandShortName;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.CommandShortName)} cannot be null!");
                }

                this.commandShortName = value;
            }
        }

        public string TurnIntoCommandString()
        {
            string returnString = this.CommandShortName;

            // todo null check for the properties
            foreach (IParameter item in this.ParameterList)
            {
                returnString += $" {item.ShortParameterName} {item.Value}";
            }

            return returnString;
        }
    }
}