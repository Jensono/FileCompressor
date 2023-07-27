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
    public class ArchiveHelpCommand : IArchiveCommand
    {
        private FixedVariables fixedVariables;

        public ArchiveHelpCommand()
        {
            this.FixedVariables = new FixedVariables();
        }

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

       

        public bool Execute()
        {
            Console.WriteLine(this.FixedVariables.HelpCommandString);
            return true;
        }
    }
}