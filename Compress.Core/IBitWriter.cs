using System;
using System.Collections.Generic;
using System.Text;

namespace Compress.Core
{
    public interface IBitWriter
    {
        void Write(ulong code, int bitCount);

        byte[] GetRoundBytes();

        byte[] GetAllBytes();
    }
}
