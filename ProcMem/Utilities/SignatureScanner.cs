using System.Collections.Generic;

namespace ProcMem.Utilities
{
    public static unsafe class SignatureScanner
    {
        public static IEnumerable<int> Scan(byte[] data, byte[] signature, byte unknownByte)
        {
            var endPoint = data.Length - signature.Length - 1;
            var sigByte = signature[0];
            var sigLength = signature.Length;

            var offsets = new List<int>();

            fixed (byte* ptrData = data, ptrSignature = signature)
            {
                for (int i = 0; i < endPoint; i++)
                {
                    if (sigByte.Equals(ptrData[i]))
                    {
                        if (SequenceEquals(ptrData, ptrSignature, unknownByte, i, sigLength))
                        {
                            offsets.Add(i);
                        }
                    }
                }
            }

            return offsets;
        }

        private static bool SequenceEquals(byte* ptrData, byte* ptrSignature, byte unknownByte, int index, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (ptrSignature[i] == unknownByte) continue;
                if (ptrSignature[i] != ptrData[index + i]) return false;
            }

            return true;
        }
    }
}
