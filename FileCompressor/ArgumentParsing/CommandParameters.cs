namespace FileCompressor
{
    public class CommandParameters
    {


        public string Source { get; set; }
        public string Destination { get; set; }

        public int WaitTime { get; set; }
        public int Retries { get; set; }

    public ICompressionAlgorithm CompressionAlgorithm { get; set; }

        //dont check these strings, if there false errorcodes will be thrown in the respective commands anyways, nobody can know if a given path is valid or not before executing a command , becouse paths can change during runtime.
        // THE COMPRESSIONALGO also doesnt need validation, it is null by defoult and only changes when the given command is create so leave it as is.
        public CommandParameters(string source,string destination = null,ICompressionAlgorithm usedCompressionAlgorithm = null,int waitTime= 1,int retries = 0 ) 
        {
            this.Source = source;
            this.Destination = destination;
            this.CompressionAlgorithm = usedCompressionAlgorithm;
            this.WaitTime = waitTime;
            this.Retries = retries;
        }

    }
}