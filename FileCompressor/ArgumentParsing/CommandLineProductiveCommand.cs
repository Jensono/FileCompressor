//-----------------------------------------------------------------------
// <copyright file="CommandLineProductiveCommand.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is on form of Command line argument. It specifies what parameters are nessacary and which are optional to a command. It also sets the calling of the command that the user has to use.
// Further, does it contain an action that is performed if the command should be executed.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class is on form of Command line argument. It specifies what parameters are necessary and which are optional to a command. It also sets the calling of the command that the user has to use.
    /// Further, does it contain an action that is performed if the command should be executed.
    /// </summary>
    public class CommandLineProductiveCommand : ICommandLineCommand
    {
        /// <summary>
        /// The field for the Action that tries to execute the given command.
        /// </summary>
        private Action<List<IParameter>> executeCommandAction;

        /// <summary>
        /// The field for the List of <see cref="IParameter"/> that are optionally added to the command.
        /// </summary>
        private List<IParameter> optionalParameters;

        /// <summary>
        /// The field for the List of <see cref="IParameter"/> that are required to execute the command.
        /// </summary>
        private List<IParameter> requiredParameters;

        /// <summary>
        /// The field for the long name or calling of the command. Is used by the user to invoke the command in the command line.
        /// </summary>
        private string longCommandArgument;

        /// <summary>
        /// The field for the short name or calling of the command. Is used by the user to invoke the command in the command line.
        /// </summary>
        private string shortCommandArgument;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineProductiveCommand"/> class. 
        /// </summary>
        /// <param name="shortCommand"> The short for name for the command. Usually starts with a '-' .</param>
        /// <param name="longCommand"> The long for name for the command. Usually starts with a '--' .</param>
        /// <param name="executionAction"> The Action that should be performed when the command is executed and called.</param>
        /// <param name="allowedOptionalParameters"> The Parameters that can be voluntarily added to the command.</param>
        /// <param name="requiredOptionalParameters"> The parameters that are required for the command to execute.</param>
        public CommandLineProductiveCommand(
            string shortCommand,
            string longCommand,
            Action<List<IParameter>> executionAction,
            List<IParameter> allowedOptionalParameters,
            List<IParameter> requiredOptionalParameters)
        {
            this.ShortCommandName = shortCommand;
            this.LongCommandName = longCommand;
            this.ExecuteCommandAction = executionAction;
            this.OptionalParameters = allowedOptionalParameters;
            this.RequiredParamters = requiredOptionalParameters;
        }

        /// <summary>
        /// Gets or sets the Action that tries to execute the given command.
        /// </summary>
        /// <value> The action to execute this command. </value>
        public Action<List<IParameter>> ExecuteCommandAction
        {
            get
            {
                return this.executeCommandAction;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ExecuteCommandAction)} cannot be null!");
                }

                this.executeCommandAction = value;
            }
        }

        /// <summary>
        /// Gets or sets the List of <see cref="IParameter"/> that are optionally added to the command.
        /// </summary>
        /// <value> The list of <see cref="IParameter"/> optional parameters for the command. </value>
        public List<IParameter> OptionalParameters
        {
            get
            {
                return this.optionalParameters;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.OptionalParameters)} cannot be null!");
                }

                this.optionalParameters = value;
            }
        }

        /// <summary>
        /// Gets or sets the List of <see cref="IParameter"/> that are required to execute the command.
        /// </summary>
        /// <value> The list of <see cref="IParameter"/> required parameters for the command. </value>
        public List<IParameter> RequiredParamters
        {
            get
            {
                return this.requiredParameters;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.RequiredParamters)} cannot be null!");
                }

                this.requiredParameters = value;
            }
        }

        /// <summary>
        /// Gets or sets the long name or calling of the command. Is used by the user to invoke the command in the command line.
        /// </summary>
        /// <value> The long command name or calling of the command. </value>
        public string LongCommandName
        {
            get
            {
                return this.longCommandArgument;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.LongCommandName)} cannot be null!");
                }

                this.longCommandArgument = value;
            }
        }

        /// <summary>
        /// Gets or sets the short name or calling of the command. Is used by the user to invoke the command in the command line.
        /// </summary>
        /// <value> The short command name or calling for the command.</value>
        public string ShortCommandName
        {
            get
            {
                return this.shortCommandArgument;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ShortCommandName)} cannot be null!");
                }

                this.shortCommandArgument = value;
            }
        }
    }
}