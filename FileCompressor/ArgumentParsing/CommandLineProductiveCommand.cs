

namespace FileCompressor
{
    using System;
    using System.Collections.Generic;

    public class CommandLineProductiveCommand : ICommandLineCommand
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
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ExecuteCommandAction)} cannot be null!");
                }

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
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.OptionalParameters)} cannot be null!");
                }

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
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.RequiredParamters)} cannot be null!");
                }

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
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.LongCommandName)} cannot be null!");
                }

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
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ShortCommandName)} cannot be null!");
                }

                this.shortCommandArgument = value;
            }
        }
    }
}