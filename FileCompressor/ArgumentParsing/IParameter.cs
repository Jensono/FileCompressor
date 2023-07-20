using System;

namespace FileCompressor
{
    public interface IParameter
    {
        string LongParameterName { get; set; }
        string ShortParameterName { get; set; }

        Func<string[],bool> CheckParameterSpecificationForValidity { get;  set; }

        

        Func<string[], object> ParseArgumentSpecificationAsValue { get; set; }

        ///TODO a parameter either must,can or doesnt support a argument after itself, the argument itself ALWAYS is a specific type eg. string for paths or int for number of retries and waittime.
        /// There are other restritions too like a retries only support numbers between 1 and 10 , same for wait , 
        /// 
        //build in a Func that returns true or false to check to see if a string can be used as a valid parameters
        // For wait for example check to see if the string is parsable as an int, if it is check if the int is between 1 and 10
    }
}