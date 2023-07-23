using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    public class ArchiveHelpCommand : IArchiveCommand
    {
        FixedVariables FixedVariables { get; set; }
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
