using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public interface ICryptoPacker
    {
        byte[] Pack(byte[] data);

        byte[] Pack(byte[] data, int offset, int length);

        byte[] GetLastSegment();

        ICryptoPacker Clean();
    }
}
