using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
    //command that actually does something
    class CommandLineProductiveCommand : ICommandLineCommand
    {
        private Action<CommandParameters> executeCommandAction;
        private List<OptionalParameterInformation> optionalParameters;
        private List<RequiredParameterInformation> requiredParameters;
        private string longCommandArgument;
        private string shortCommandArgument;

        public CommandLineProductiveCommand(
            string shortCommand,
            string longCommand,
            Action<CommandParameters> executionAction,
            List<OptionalParameterInformation> allowedOptionalParameters,
            List<RequiredParameterInformation> requiredOptionalParameters)
        {
            this.ShortCommandName = shortCommand;
            this.LongCommandName = longCommand;
            this.ExecuteCommandAction = executionAction;
            this.OptionalParameters = allowedOptionalParameters;
            this.RequiredParamters = requiredOptionalParameters;
        }

        // null checks maybe also 
        public Action<CommandParameters> ExecuteCommandAction
        {
            get
            {
                return this.executeCommandAction;
            }
            set
            {
                this.executeCommandAction = value;
            }
        }
        public List<OptionalParameterInformation> OptionalParameters
        {
            get
            {
                return this.optionalParameters;
            }
            set
            {
                this.optionalParameters = value;
            }
        }
        public List<RequiredParameterInformation> RequiredParamters
        {
            get
            {
                return this.requiredParameters;
            }
            set
            {
                this.requiredParameters = value;
            }
        }
        public string LongCommandName
        {
            get
            {
                return this.longCommandArgument;
            }
            set
            {
                this.longCommandArgument = value;
            }
        }
        public string ShortCommandName
        {
            get
            {
                return this.shortCommandArgument;
            }
            set
            {
                this.shortCommandArgument = value;
            }
        }
    }
}
