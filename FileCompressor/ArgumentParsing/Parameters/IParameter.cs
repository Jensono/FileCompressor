

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