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

    public interface ICommandLineCommand
    {
        Action<List<IParameter>> ExecuteCommandAction { get; set; }

        List<IParameter> OptionalParameters { get; set; }
        List<IParameter> RequiredParamters { get; set; }

        string LongCommandName { get; set; }
        string ShortCommandName { get; set; }
    }
}