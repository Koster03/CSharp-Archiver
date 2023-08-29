using System;
using System.IO;

namespace Compress.Core
{
    public interface ICryptoStreamTransporter
    {
        void Pack(Stream input, Stream output, long length, ICryptoPacker packer);

        void Unpack(Stream input, Stream output, long length, ICryptoUnpacker unpacker);

        int Repeat { get; }

        event EventHandler<DataProcessedEventArgs> DataProcessed;
    }
}
