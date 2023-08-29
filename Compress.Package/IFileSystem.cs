using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    // represents interaction with file system
    public interface IFileSystem
    {
        bool FileExists(string path);

        bool DirectoryExists(string path);

        bool IsDirectory(string path);

        void Delete(string path);

        void Move(string from, string to);

        void CreateDirectory(string path);

        Stream Open(string path, FileMode mode);

        DateTime GetLastWriteTimeUtc(string path);

        long GetFileLength(string path);

        IList<string> GetChildItems(string path);

        void CreateTextFile(string path, string content);

        string ReadTextFile(string path);
    }
}
