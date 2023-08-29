using System.Collections.Generic;

namespace Compress.Package
{
    public abstract class PackageWriterTask
    {
        public ItemHeader OutputItem { get; set; }
    }

    public class CopyTask : PackageWriterTask
    {
        public CopyTask(ItemHeader ih)
        {
            Item = ih;
        }
        public ItemHeader Item { get; set; }
    }

    public class PackTask : PackageWriterTask
    {
        public PackTask(string externalPath, string internalPath, ItemType type)
        {
            Path = new ExternalInternalPath();
            Path.ExternalPath = externalPath;
            Path.InternalPath = internalPath;
            Path.Type = type;
        }
        public ExternalInternalPath Path { get; set; }
    }

    public interface IPackageWriter
    {
        // write new package on the base of tasks list
        // tasks includes
        //      CopyTask means copy part of old package into new palce in new package
        //      PackTask means packing of file
        PackageHeader Write(string newPackageName, string oldPackageName, List<PackageWriterTask> tasks, System.Action<FileProcessingEventArgs> fileProcessing);
    }
}