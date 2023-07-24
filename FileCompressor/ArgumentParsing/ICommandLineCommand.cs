﻿using System;
using System.Collections.Generic;

namespace FileCompressor
{
    public interface ICommandLineCommand
    {
        
        Action<List<IParameter>> ExecuteCommandAction { get; set; }

        List<IParameter> OptionalParameters { get; set; }
        List<IParameter> RequiredParamters { get; set; }

        string LongCommandName { get; set; }
        string ShortCommandName { get; set; }
    }
}