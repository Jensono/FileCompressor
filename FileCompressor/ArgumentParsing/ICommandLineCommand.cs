//-----------------------------------------------------------------------
// <copyright file="ICommandLineCommand.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is the interface for all the commands used with given user input. It contains a list of parameters that are required for the command and one that can be optionally added if so requireb by the user.
// The interface also contains a Action that validates given parameters and executes the actual command. It also contains information on what kind of phrase the user has to use to call it.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///  This class is the interface for all the commands used with given user input. It contains a list of parameters that are required for the command and one that can be optionally added if so required by the user.
    /// The interface also contains a Action that validates given parameters and executes the actual command. It also contains information on what kind of phrase the user has to use to call it.
    /// </summary>
    public interface ICommandLineCommand
    {
        /// <summary>
        /// Gets or sets the Action that tries to execute the given command.
        /// </summary>
        /// <value> The action to execute this command. </value>
        Action<List<IParameter>> ExecuteCommandAction { get; set; }

        /// <summary>
        /// Gets or sets the List of <see cref="IParameter"/> that are optionally added to the command.
        /// </summary>
        /// <value> The list of <see cref="IParameter"/> optional parameters for the command. </value>
        List<IParameter> OptionalParameters { get; set; }

        /// <summary>
        /// Gets or sets the List of <see cref="IParameter"/> that are required to execute the command.
        /// </summary>
        /// <value> The list of <see cref="IParameter"/> required parameters for the command. </value>
        List<IParameter> RequiredParamters { get; set; }

        /// <summary>
        /// Gets or sets the long name or calling of the command. Is used by the user to invoke the command in the command line.
        /// </summary>
        /// <value> The long command name or calling of the command. </value>
        string LongCommandName { get; set; }

        /// <summary>
        /// Gets or sets the short name or calling of the command. Is used by the user to invoke the command in the command line.
        /// </summary>
        /// <value> The short command name or calling for the command.</value>
        string ShortCommandName { get; set; }
    }
}