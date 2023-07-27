//-----------------------------------------------------------------------
// <copyright file="IParameter.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This interface as the base to create new parameters that can then be used for commands inside of the application.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    public interface IParameter
    {
        string LongParameterName { get; set; }
        string ShortParameterName { get; set; }

        Func<string[], bool> CheckParameterSpecificationForValidity { get; set; }

        object Value { get; set; }
        bool TryParseValueAndSetIt(string[] array);

        IParameter DeepCloneSelf();
    }
}