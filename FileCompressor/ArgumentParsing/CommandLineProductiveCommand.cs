

namespace FileCompressor
{
    using System;
    using System.Collections.Generic;

    internal class CommandLineProductiveCommand : ICommandLineCommand
    {
        private Action<List<IParameter>> executeCommandAction;
        private List<IParameter> optionalParameters;
        private List<IParameter> requiredParameters;
        private string longCommandArgument;
        private string shortCommandArgument;

        public CommandLineProductiveCommand(
            string shortCommand,
            string longCommand,
            Action<List<IParameter>> executionAction,
            List<IParameter> allowedOptionalParameters,
            List<IParameter> requiredOptionalParameters)
        {
            this.ShortCommandName = shortCommand;
            this.LongCommandName = longCommand;
            this.ExecuteCommandAction = executionAction;
            this.OptionalParameters = allowedOptionalParameters;
            this.RequiredParamters = requiredOptionalParameters;
        }

        // null checks maybe also
        public Action<List<IParameter>> ExecuteCommandAction
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

        public List<IParameter> OptionalParameters
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

        public List<IParameter> RequiredParamters
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