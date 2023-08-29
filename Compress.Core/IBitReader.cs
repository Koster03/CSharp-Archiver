using System;
using System.Collections.Generic;
using System.Text;

namespace Compress.Core
{
    public interface IBitReader
    {
        bool TryRead(int bitCount, out ulong res);

        void PutBytes(byte[] data);
    }
}
