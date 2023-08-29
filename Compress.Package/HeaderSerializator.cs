using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public partial class HeaderSerializator
    {
        public PackageHeader Load(Stream input)
        {
            input.Position = 0;
            int count = LoadInt(input);
            ItemHeader[] item = new ItemHeader[count];

            for (int i = 0; i < count; i++)
            {
                item[i] = LoadItemHeader(input);
            }

            var h = new PackageHeader();
            //h.Items = item.ToList();
            //h.ReconstructInternalRelations(5);
            return new PackageHeader
            {
                Items = item.ToList(),
            };
        }

        private void RecountOffset(Stream output, Dictionary<long,long> dic)
        {
            long startOffset = output.Position;

            output.Position = dic.ElementAt(0).Key;
            SaveLong(output, startOffset);

            for (int i = 1; i < dic.Count; i++)
            {
                output.Position = dic.ElementAt(i).Key;
                startOffset += dic.ElementAt(i - 1).Value;
                SaveLong(output, startOffset);
            }
        }

        public void Save(Stream output, PackageHeader header)
        {
            SaveInt(output, Convert.ToInt32(header.Items.Count));
            foreach (var item in header.Items)
            {
                SaveItemHeader(output, item);
            }
        }

        private ItemHeader LoadItemHeader(Stream input)
        {
            if (LoadInt(input) == Convert.ToInt32(ItemType.File))
            {
                return LoadFileHeader(input);
            }
            else
            {
                return LoadDirectoryHeader(input);
            }
        }

        private void SaveItemHeader(Stream output, ItemHeader itemHeader)
        {
            switch (itemHeader)
            {
                case FileHeader fh:
                    {
                        SaveFileHeader(output, fh);
                        break;
                    }
                case DirectoryHeader dh:
                    {
                        SaveDirectoryHeader(output, dh);
                        break;
                    }
            }
        }

        private FileHeader LoadFileHeader(Stream input)
        {
            int lengthString = LoadInt(input);

            return new FileHeader
            {
                Path = LoadString(input, lengthString),
                Modified = LoadDateTime(input),
                PackedLength = LoadLong(input),
                UnpackedLength = LoadLong(input),
                StartOffset = LoadLong(input),
            };
        }

        private void SaveFileHeader(Stream output, FileHeader fileHeader)
        {
            SaveInt(output, Convert.ToInt32(ItemType.File));

            SaveInt(output, Convert.ToInt32(fileHeader.Path.Length));
            SaveString(output, fileHeader.Path);
            SaveDateTime(output, fileHeader.Modified);

            SaveLong(output, fileHeader.PackedLength);
            SaveLong(output, fileHeader.UnpackedLength);

            //packLength.Add(output.Position, fileHeader.PackedLength);
            SaveLong(output, 0);
        }
        private void SaveDirectoryHeader(Stream output, DirectoryHeader itemHeader)
        {
            SaveInt(output, Convert.ToInt32(ItemType.Directory));

            SaveInt(output, Convert.ToInt32(itemHeader.Path.Length));
            SaveString(output, itemHeader.Path);
            SaveDateTime(output, itemHeader.Modified);
        }

        private DirectoryHeader LoadDirectoryHeader(Stream input)
        {
            int lengthString = LoadInt(input);

            return new DirectoryHeader
            {
                Path = LoadString(input, lengthString),
                Modified = LoadDateTime(input),
            };
        }

        private string LoadString(Stream input, int length)
        {
            buffer = new byte[length];
            input.Read(buffer, 0, length);
            return Encoding.UTF8.GetString(buffer);
        }

        private void SaveString(Stream output, string str)
        {
            foreach (var byt in Encoding.UTF8.GetBytes(str))
            {
                output.WriteByte(byt);
            }
        }

        private DateTime LoadDateTime(Stream input)
        {
            buffer = new byte[8];
            input.Read(buffer, 0, 8);
            DateTime dt = DateTime.FromBinary(BitConverter.ToInt64(buffer, 0));
            return dt;
        }

        private void SaveDateTime(Stream output, DateTime dt)
        {
            foreach (var byt in BitConverter.GetBytes(dt.Ticks))
            {
                output.WriteByte(byt);
            }
        }

        private int LoadInt(Stream input)
        {
            buffer = new byte[4];
            input.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        private void SaveInt(Stream output, int i)
        {
            //var bytes = BitConverter.GetBytes(i);
            output.Write(BitConverter.GetBytes(i), 0, 4);

            //foreach (var byt in BitConverter.GetBytes(i))
            //{
            //    output.WriteByte(byt);
            //}
        }

        private long LoadLong(Stream input)
        {
            buffer = new byte[8];
            input.Read(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        private void SaveLong(Stream output, long l)
        {
            foreach (var byt in BitConverter.GetBytes(l))
            {
                output.WriteByte(byt);
            }
        }

        //private long startOffset = 0;
        private byte[] buffer;
        //private Dictionary<long, long> packLength = new Dictionary<long, long>();
    }
}
