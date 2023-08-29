namespace Compress.Package
{
    public class FileProcessingEventArgs
    {
        public string FileName { get; internal set; }

        public FileProcessingType Type { get; internal set; }

        public long Total { get; internal set; }

        public long TotalProcessed { get; internal set; }
    }

    public enum FileProcessingType { 
        Pack,
        Unpack,
        Copy,
    };
}
