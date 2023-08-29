using System;
using System.Collections.Generic;
using System.Text;

namespace Compress.Core
{
    public class LzwAlgoParams
    {
        public int MaxCodeBitCount { get; set; }

        public static readonly LzwAlgoParams Default = new LzwAlgoParams
        {
            MaxCodeBitCount = 20,
        };
    }
}
