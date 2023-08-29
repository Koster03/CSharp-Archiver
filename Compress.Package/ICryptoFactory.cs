using Compress.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public interface ICryptoFactory
    {
        ICryptoPacker CreatePacker();

        ICryptoUnpacker CreateUnpacker();
    }
}