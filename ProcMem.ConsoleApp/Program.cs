using ProcMem.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProcMem.ConsoleApp
{
    class Program
    {
        private const string Pattern = "A4 07 15 ? 3F 08 ? ? 4C 70";
        private const int DataLength = 100 * 1024 * 1024;

        private static readonly Random Random = new Random();

        static void Main(string[] args)
        {
            var signature = ParseHelper.BytesFromPattern(Pattern, out var unknownByte);
            Console.WriteLine($"Signature generated: {string.Join(" ", signature)}");

            while (true)
            {
                var data = StartBench(() => GenerateData(DataLength), "Generating data.. ");
                var generatedOffsets = StartBench(() => InjectSignaturesToData(data, signature, unknownByte, 60).OrderBy(x => x), 
                    "Generating offsets.. ");
                var offsets = StartBench(() => SignatureScanner.Scan(data, signature, unknownByte), "Scanning offsets.. ");

                if (!Compare(generatedOffsets, offsets)) throw new Exception("Invalid scan.");
                Console.WriteLine();
            }

            Console.ReadKey(true);
        }

        private static bool Compare(IEnumerable<int> left, IEnumerable<int> right)
        {
            var l = left.ToList();
            var r = right.ToList();

            if (l.Count != r.Count) return false;

            for (var i = 0; i < l.Count; i++)
            {
                if (l[i] != r[i]) return false;
            }

            return true;
        }

        private static T StartBench<T>(Func<T> func, string message)
        {
            Console.Write(message);
            var watch = Stopwatch.StartNew();
            var result = func();
            watch.Stop();
            Console.WriteLine($"{watch.ElapsedMilliseconds} ms");
            return result;
        }

        private static byte[] GenerateData(int length)
        {
            var data = new byte[length];
            Random.NextBytes(data);
            return data;
        }

        private static IEnumerable<int> InjectSignaturesToData(byte[] data, byte[] signature, byte unknownByte, int count)
        {
            var offsets = new List<int>();

            for (int i = 0; i < count; i++)
            {
                var injectedIndex = Random.Next(signature.Length, data.Length - signature.Length);
                InjectSignatureToData(data, signature, unknownByte, injectedIndex);
                offsets.Add(injectedIndex);
            }

            return offsets;
        }

        private static void InjectSignatureToData(byte[] data, byte[] signature, byte unknownByte, int index)
        {
            for (int i = 0; i < signature.Length; i++)
            {
                var injectedByte = signature[i] != unknownByte ? signature[i] : (byte)Random.Next(0, 256);
                data[index + i] = injectedByte;
            }
        }
    }
}
