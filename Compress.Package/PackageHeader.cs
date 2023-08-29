using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public class PackageHeader
    {
        // list of all items in package, files and directories
        public List<ItemHeader> Items { get; set; } = new List<ItemHeader>();

        // represents root directory level of package
        public DirectoryHeader RootDirectory { get; set; }

        public void ReconstructInternalRelations(long startContent)
        {
            //RootDirectory = DirectoryHeader.FromDirectory(@"C:\Windows\System32");

            long offset = startContent;

            itemDic = new Dictionary<string, ItemHeader>();

            foreach (var item in Items)
            {
                //item.Path = string.Concat(RootDirectory.Path, "\\", item.Path);
                itemDic.Add(item.Path, item);
            }

            foreach (var item in Items)
            {
                DirectoryHeader it = new DirectoryHeader();

                switch (item)
                {
                    case FileHeader fh:
                        {
                            fh.StartOffset = offset;
                            offset += fh.PackedLength;

                            var parents = Path.GetDirectoryName(item.Path);
                            it = (DirectoryHeader)GetItemForPath(parents);
                            break;
                        }
                    case DirectoryHeader dh:
                        {
                            var parents = Path.GetDirectoryName(item.Path);

                            it = (DirectoryHeader)GetItemForPath(parents);
                            break;
                        }
                }

                if (it == null)
                {
                    RootDirectory.Items.Add(item);
                    item.Directory = RootDirectory;
                }
                else
                {
                    it.Items.Add(item);
                    item.Directory = it;
                }
            }
        }

        // return item with Path == 'path' otherwise null
        public ItemHeader GetItemForPath(string path)
        {
            if (itemDic.TryGetValue(path, out var item))
                return item;
            else
                return null;
        }

        private Dictionary<string, ItemHeader> itemDic;
    }

    public abstract class ItemHeader
    {
        public string Path { get; set; }

        public DateTime Modified { get; set; }

        public DirectoryHeader Directory { get; set; }

        public virtual ItemHeader Clone()
        {
            return (ItemHeader)this.MemberwiseClone();
        }
    }

    public class FileHeader : ItemHeader
    {
        public long PackedLength { get; set; }

        public long UnpackedLength { get; set; }

        public long StartOffset { get; set; }
    }

    public class DirectoryHeader : ItemHeader
    {
        public static DirectoryHeader FromDirectory(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            return new DirectoryHeader
            {
                Path = System.IO.Path.GetFileName(path),
                Modified = directoryInfo.LastWriteTimeUtc,
            };
        }

        public List<ItemHeader> Items { get; set; } = new List<ItemHeader>();
    }
}
