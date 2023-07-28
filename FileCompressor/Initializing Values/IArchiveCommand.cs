﻿//-----------------------------------------------------------------------
// <copyright file="IArchiveCommand.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class acts as the interfaced used inside all archiver Execution commands.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    /// <summary>
    /// This class acts as the interfaced used inside all archive Execution commands.
    /// </summary>
    public interface IArchiveCommand
    {
        bool Execute();
    }
}