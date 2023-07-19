using System;
using System.Collections.Generic;

namespace FileCompressor
{
    public interface ICommandLineCommand
    {
        
        Action<CommandParameters> ExecuteCommandAction { get; set; }

        List<OptionalParameterInformation> OptionalParameters { get; set; }
        List<RequiredParameterInformation> RequiredParamters { get; set; }

        string LongCommandName { get; set; }
        string ShortCommandName { get; set; }
    }
}