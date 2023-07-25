using System;

namespace FileCompressor
{
    public interface IParameter
    {
        string LongParameterName { get; set; }
        string ShortParameterName { get; set; }

        Func<string[], bool> CheckParameterSpecificationForValidity { get; set; }

        bool TryParseValueAndSetIt(string[] array);

        object Value { get; set; }

        IParameter DeepCloneSelf();
    }
}