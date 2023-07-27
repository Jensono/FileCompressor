﻿//-----------------------------------------------------------------------
// <copyright file="DestinationParameter.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is used to save infomration on the destination of a file or directory.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    public class DestinationParameter : IParameter
    {
        private string shortParameterArgument;
        private string longParameterArgument;
        private Func<string[], bool> checkFunctionForParameterValidity;
        private object value;      

        public DestinationParameter(string shortCommandName, string longCommandName)
        {
            this.LongParameterName = longCommandName;
            this.ShortParameterName = shortCommandName;
            this.CheckParameterSpecificationForValidity = (parameter) =>
            {
                if (parameter.Length >= 2)
                {
                    return false;
                }

                if (parameter.Length == 0)
                {
                    return false;
                }

                // no additional validation can be done becouse souce and destination can be very diffrent things depending on the command,
                // they can be filenames, directories, i could check if it is a valid path or not but the commands do that anyway.
                // also commands can have paths in them that will only be created after the command entered runtime which makes
                // it impossible to confirm if a path is valid or not before excectuion and runtime.
                return true;
            };
        }

        public string LongParameterName
        {
            get
            {
                return this.longParameterArgument;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.LongParameterName)} cannot be null!");
                }

                this.longParameterArgument = value;
            }
        }

        public string ShortParameterName
        {
            get
            {
                return this.shortParameterArgument;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ShortParameterName)} cannot be null!");
                }

                this.shortParameterArgument = value;
            }
        }

        public Func<string[], bool> CheckParameterSpecificationForValidity
        {
            get
            {
                return this.checkFunctionForParameterValidity;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.CheckParameterSpecificationForValidity)} cannot be null!");
                }

                this.checkFunctionForParameterValidity = value;
            }
        }

        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.value)} can not be null!");
                }

                if (value != null && !(value is string))
                {
                    throw new ArgumentException($"{nameof(this.value)} must be a string!");
                }

                this.value = value;
            }
        }

        public bool TryParseValueAndSetIt(string[] argumentArray)
        {
            if (this.CheckParameterSpecificationForValidity(argumentArray))
            {
                this.Value = argumentArray[0];
                return true;
            }

            return false;
        }

        public IParameter DeepCloneSelf()
        {
            return new DestinationParameter(this.ShortParameterName, this.LongParameterName);
        }
    }
}