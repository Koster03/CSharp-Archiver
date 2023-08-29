using System;
using System.Collections.Generic;
using System.Text;

namespace Compress.Core
{
    enum SpecialSeqCodes : ulong
    {
        Clear = 1 << 8,
        Eof,
        NotFound = ulong.MaxValue,
    }

    public interface ISequenceTable
    {
        void Init();

        ulong AddSequence(ulong previousSecCode, byte ch);

        ulong FindSequence(ulong previousSecCode, byte ch);

        byte[] GetSequence(ulong code);

        int CurrentBitLenght { get; }

        int NextBitLenght { get; }
    }
}
