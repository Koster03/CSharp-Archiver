using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public class PathHelper
    {
        public static bool IsBaseOf(string root, string child)
        {
            var directoryPath = EndsWithSeparator(child);
            var rootPath = EndsWithSeparator(root);
            return directoryPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase);
        }

        private static string EndsWithSeparator(string path)
        {
            return RemoveEndSeparator(path) + Path.DirectorySeparatorChar;
        }

        public static string RemoveEndSeparator(string path)
        {
            return path?.TrimEnd('/', '\\');
        }
    }
}
