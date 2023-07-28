//-----------------------------------------------------------------------
// <copyright file="ArchiveHelpCommand.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is the help command. When used it displays instrutions on how to use the application to the consol.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;

    /// <summary>
    /// This class is the help command. When used it displays instructions on how to use the application to the console.
    /// </summary>
    public class ArchiveHelpCommand : IArchiveCommand
    {
        /// <summary>
        /// The field for the <see cref="FixedVariables"/> that are used inside the class.
        /// </summary>
        private FixedVariables fixedVariables;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveHelpCommand"/> class. 
        /// </summary>
        public ArchiveHelpCommand()
        {
            this.FixedVariables = new FixedVariables();
        }

        /// <summary>
        /// Gets or sets the <see cref="FixedVariables"/> that are used inside the class.
        /// </summary>
        /// <value> The <see cref="FixedVariables"/> that are used inside the class. </value>
        public FixedVariables FixedVariables
        {
            get
            {
                return this.fixedVariables;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.FixedVariables)} cannot be null!");
                }

                this.fixedVariables = value;
            }
        }

        /// <summary>
        /// This method executes the help command, it prints the help string to the console which contains all the information needed to operate the application.
        /// </summary>
        /// <returns> A boolean indicating whether or not the operation succeeded.</returns>
        public bool Execute()
        {
            Console.WriteLine(this.FixedVariables.HelpCommandString);
            return true;
        }
    }
}