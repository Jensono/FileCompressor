//-----------------------------------------------------------------------
// <copyright file="ArchiveErrorCodeException.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is the Exception that is thrown if any Error, that is associated with the archive, occurs.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    public class ArchiveErrorCodeException : Exception
    {
        private string errorCode;

        public ArchiveErrorCodeException(string errorcodeText)
        {
            this.ErrorCode = errorcodeText;
        }

        public string ErrorCode
        {
            get
            {
                return this.errorCode;
            }

            private set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ErrorCode)} cannot be null!");
                }

                this.errorCode = value;
            }
        }

        

        public void AppendErrorCodeInformation(string additionalInformation)
        {
            this.ErrorCode += additionalInformation;
        }
    }
}