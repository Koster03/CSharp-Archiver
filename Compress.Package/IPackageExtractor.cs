using System;
using System.IO;

namespace Compress.Package
{
    public interface IPackageExtractor
    {
        // extract files from package in @packageStream into pathTo
        // (multiple files because @item could be directory which contains more than one files)
        // @shouldOverwrite delegate is used to determine should particular file be overwritten
        void ExtractFileTo(Stream packageStream, ItemHeader item, string pathTo, Func<string, bool> shouldOverwrite, Action<FileProcessingEventArgs> fileProcessing);
    }
}