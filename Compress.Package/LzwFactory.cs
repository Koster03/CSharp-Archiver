using Compress.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Compress.Package
{
    public class LzwFactory : ICryptoFactory
    {
        public LzwFactory(int keyLength = 20)
        {
            this.keyLength = keyLength;
        }

        public ICryptoPacker CreatePacker()
        {
            return new LzwPacker(new LzwAlgoParams { MaxCodeBitCount = this.keyLength });
        }

        public ICryptoUnpacker CreateUnpacker()
        {
            return new LzwUnpacker();
        }

        private int keyLength;
    }
}