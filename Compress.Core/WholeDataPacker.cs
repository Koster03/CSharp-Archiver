using System.IO;

namespace Compress.Core
{
    public abstract class WholeDataPacker : ICryptoPacker
    {
        abstract public byte[] Pack(byte[] data);

        public byte[] Pack(byte[] data, int offset, int length)
        {
            this.ms.Write(data, offset, length);
            return new byte[0];
        }

        public byte[] GetLastSegment()
        {
            return this.Pack(this.ms.ToArray());
        }

        public ICryptoPacker Clean()
        {
            throw new System.NotImplementedException();
        }

        private MemoryStream ms = new MemoryStream();
    }
}