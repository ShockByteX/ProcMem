using System.Collections.Generic;

namespace ProcMem.Utilities
{
    public static unsafe class SignatureScanner
    {
        public static IEnumerable<int> Scan(byte[] data, string pattern, bool firstOnly = true)
        {
            var signature = ParseHelper.BytesFromPattern(pattern, out var unknownByte);

            return Scan(data, signature, unknownByte, firstOnly);
        }

        public static IEnumerable<int> Scan(byte[] data, byte[] signature, byte unknownByte, bool firstOnly = true)
        {
            var endPoint = data.Length - signature.Length - 1;
            var sigByte = signature[0];
            var sigLength = signature.Length;

            var offsets = new List<int>();

            fixed (byte* ptrData = data, ptrSignature = signature)
            {
                for (var i = 0; i < endPoint; i++)
                {
                    if (!sigByte.Equals(ptrData[i])) continue;
                    if (!SequenceEquals(ptrData, ptrSignature, unknownByte, i, sigLength)) continue;

                    offsets.Add(i);

                    if (firstOnly) return offsets;
                }
            }

            return offsets;
        }

        private static bool SequenceEquals(byte* ptrData, byte* ptrSignature, byte unknownByte, int index, int length)
        {
            for (var i = 0; i < length; i++)
            {
                if (ptrSignature[i] == unknownByte) continue;
                if (ptrSignature[i] != ptrData[index + i]) return false;
            }

            return true;
        }
    }
}
