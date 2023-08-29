using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public interface ICryptoUnpacker
    {
        byte[] Unpack(byte[] data);

        byte[] Unpack(byte[] packed, int offset, int length);

        ICryptoUnpacker Clean();
    }
}
