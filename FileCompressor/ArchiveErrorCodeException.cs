using System;
using System.Runtime.Serialization;

namespace FileCompressor
{
    
    public class ArchiveErrorCodeException : Exception
    {
        //todo checks?
        public string ErrorCode { get; private set; }
        public ArchiveErrorCodeException(string errorcodeText)
        {
            this.ErrorCode = errorcodeText;
        }

        public void AppendErrorCodeInformation(string additionalInformation)
        {
            this.ErrorCode += additionalInformation;

        }

    }
}