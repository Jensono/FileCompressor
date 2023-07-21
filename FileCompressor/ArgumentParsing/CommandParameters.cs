using System.Collections.Generic;

namespace FileCompressor
{
    public class CommandParameters
    {

        //todo checks and shit
        public List<IParameter> ParameterList { get; set; }
        public string CommandShortName { get; set; }


    public ICompressionAlgorithm CompressionAlgorithm { get; set; }

        //dont check these strings, if there false errorcodes will be thrown in the respective commands anyways, nobody can know if a given path is valid or not before executing a command , becouse paths can change during runtime.
        // THE COMPRESSIONALGO also doesnt need validation, it is null by defoult and only changes when the given command is create so leave it as is.
        public CommandParameters(List<IParameter> parameterList,string commandShortName ) 
        {
            this.ParameterList = parameterList;
            this.CommandShortName = commandShortName;

    }
}