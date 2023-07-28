//-----------------------------------------------------------------------
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
        /// <summary>
        /// Every Archive must be executable so the command can be executed.
        /// </summary>
        /// <returns> A boolean value indicating whether or not the execution was succeful.</returns>
        bool Execute();
    }
}