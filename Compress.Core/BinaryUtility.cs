using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Compress.Core
{
    public class BinaryUtility
    {
        public static ulong[] ConvertToUlong(byte[] data)
        {
            var size = data.Length / sizeof(ulong);
            var codes = new ulong[size];
            for (var index = 0; index < size; index++)
            {
                codes[index] = BitConverter.ToUInt64(data, index * sizeof(ulong));
            }

            return codes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetRightByteWithShift(ulong number, int shift)
        {
            byte res;
            if (shift < 0)
                res = (byte)((number << -shift) & 0xFF);
            else if (shift > 0)
                res = (byte)((number >> shift) & 0xFF);
            else
                res = (byte)(number & 0xFF);

            return res;
        }
    }
}
