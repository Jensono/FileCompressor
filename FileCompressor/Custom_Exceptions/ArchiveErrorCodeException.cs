

namespace FileCompressor
{
    using System;
    public class ArchiveErrorCodeException : Exception
    {
        private string errorCode;

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