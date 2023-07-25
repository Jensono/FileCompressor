using System;

namespace FileCompressor
{
    public class ArchiveHelpCommand : IArchiveCommand
    {
        private FixedVariables FixedVariables { get; set; }

        public ArchiveHelpCommand()
        {
            this.FixedVariables = new FixedVariables();
        }

        public bool Execute()
        {
            Console.WriteLine(this.FixedVariables.HelpCommandString);
            return true;
        }
    }
}