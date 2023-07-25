

namespace FileCompressor
{
    using System;
    public class ArchiveHelpCommand : IArchiveCommand
    {
        private FixedVariables fixedVariables;

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