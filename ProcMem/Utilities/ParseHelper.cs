using System;
using System.Linq;

namespace ProcMem.Utilities
{
    public static class ParseHelper
    {
        public static readonly string[] BytesArray = new string[256];

        static ParseHelper()
        {
            for (var i = 0; i < BytesArray.Length; i++)
            {
                var strByte = i.ToString("X");

                if (strByte.Length == 1)
                {
                    BytesArray[i] = $"0{strByte}";
                }
                else
                {
                    BytesArray[i] = strByte;
                }
            }
        }

        public static byte[] BytesFromPattern(string pattern, out byte unknownByte)
        {
            var arrBytes = pattern.Split(' ');
            unknownByte = 0x00;

            for (byte i = 0; i < BytesArray.Length; i++)
            {
                if (!arrBytes.Contains(BytesArray[i]))
                {
                    unknownByte = i;
                    break;
                }
            }

            var signature = new byte[arrBytes.Length];

            for (var i = 0; i < arrBytes.Length; i++)
            {
                if (arrBytes[i] == "?")
                {
                    signature[i] = unknownByte;
                }
                else
                {
                    signature[i] = Convert.ToByte(arrBytes[i], 16);
                }
            }

            return signature;
        }
    }
}