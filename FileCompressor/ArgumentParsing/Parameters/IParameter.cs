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

    /// <summary>
    /// This interface is the base to create new parameters that can then be used for commands inside of the application.
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// Gets or sets the long name or calling of the parameter, that is also used by the user in the command line.
        /// </summary>
        /// <value> The long name or calling for the parameter. </value>
        string LongParameterName { get; set; }

        /// <summary>
        /// Gets or sets the short name or calling of the parameter, that is also used by the user in the command line.
        /// </summary>
        /// <value> The short name or calling for the parameter. </value>
        string ShortParameterName { get; set; }

        /// <summary>
        /// Gets or sets the Function that checks whether or not a parameter specification is valid.
        /// </summary>
        /// <value> The Function that checks if the parameter specification are fulfilled. </value>
        Func<string[], bool> CheckParameterSpecificationForValidity { get; set; }

        /// <summary>
        /// Gets or sets the value the parameter holds.
        /// </summary>
        /// <value> The value of the parameter that is saved inside the class. </value>
        object Value { get; set; }

        bool TryParseValueAndSetIt(string[] array);

        IParameter DeepCloneSelf();
    }
}