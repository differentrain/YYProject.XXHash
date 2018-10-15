using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using YYProject.XXHash;

namespace ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            //1KB~100MB
            MainTest(100, 1024, 1024 * 1024 * 100 + 1, true);
            Console.ReadLine();
        }


        private static readonly Random MyRnd = new Random();
        private static readonly XXHash32 xxH32 = new XXHash32();
        private static readonly XXHash64 xxH64 = new XXHash64();
        private static readonly MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        private static readonly Stopwatch watch = new Stopwatch();
        private static readonly Crc32C Crc32Normal = new Crc32C();

        /*
         * If your CPU does not support SSE4.2 instruction set, 
         * remove the conditional compilation symbol "SSE42".
         */
#if SSE42
        private static readonly Crc32CSSE42 Crc32SSE = new Crc32CSSE42();
#endif

        private static readonly string Platform = IntPtr.Size == 4 ? "x86" : "x64";

        private static ulong TotalSize;
        private static int TotalCount;
        private static decimal XH32Cost;
        private static decimal XH64Cost;
        private static decimal MD5Cost;
        private static decimal CRCNormalCost;
#if SSE42
        private static decimal CRCSSECost;
#endif


        private static void MainTest(int count, int minFileSize, int maxFileSize, bool testFile)
        {
            TotalSize = 0;
            XH32Cost = 0;
            XH64Cost = 0;
            MD5Cost = 0;
            CRCNormalCost = 0;
#if SSE42
            CRCSSECost = 0;
#endif

            TotalCount = count;

            Console.WriteLine(testFile ? "File hash test" : "Bytes hash test");

            for (int i = 0; i < TotalCount; i++)
            {
                //1KB~100MB
                if (testFile)
                {
                    TestFile(i + 1, MyRnd.Next(minFileSize, maxFileSize));
                }
                else
                {
                    TestBytes(i + 1, MyRnd.Next(minFileSize, maxFileSize));
                }


            }
            Console.WriteLine("=============================================================");
#if DEBUG
            Console.WriteLine($"{(testFile ? "File" : "Bytes")} hash test on {Platform} debug");
#else
            Console.WriteLine($"{(testFile ? "File" : "Bytes")} hash test on {Platform} release");
#endif
            Console.WriteLine();
            Console.WriteLine($"Min size={minFileSize}, max size={maxFileSize}");
            Console.WriteLine();
            Console.WriteLine($"Total size: {TotalSize}");
            Console.WriteLine($"Total count: {TotalCount}");
            Console.WriteLine($"xxHash32 total cost: {XH32Cost}");
            Console.WriteLine($"xxHash64 total cost: {XH64Cost}");
            Console.WriteLine($"MD5 total cost: {MD5Cost}");
            Console.WriteLine($"CRC32C normal total cost: {CRCNormalCost}");
#if SSE42
            Console.WriteLine($"CRC32C SSE42 total cost: {CRCSSECost}");
#endif
            Console.WriteLine();
            Console.WriteLine($"Average size: {(decimal)TotalSize / TotalCount}");
            Console.WriteLine($"xxHash32 average cost: {XH32Cost / TotalCount}");
            Console.WriteLine($"xxHash64 average cost: {XH64Cost / TotalCount}");
            Console.WriteLine($"MD5 average cost: {MD5Cost / TotalCount}");
            Console.WriteLine($"CRC32C normal average cost: {CRCNormalCost / TotalCount}");
#if SSE42
            Console.WriteLine($"CRC32C SSE42 average cost: {CRCSSECost / TotalCount}");
#endif
            Console.WriteLine();
            Console.WriteLine("All Test Complete.");
        }



        private static void TestFile(int index, int fileSize)
        {
            TotalSize += (ulong)fileSize;
            Console.WriteLine("=============================================================");
            var path = CreateFile(fileSize);
            Console.WriteLine($"No.{index}, File size = {fileSize.ToString("X")}");
            Console.WriteLine();
            Console.WriteLine($"Official tool xxHash32: {GetXXHashCPortOutput(path, 0)}");
            Console.WriteLine($"Official tool xxHash64: {GetXXHashCPortOutput(path, 1)}");
            Console.WriteLine();
            using (var fs = new FileStream(path, FileMode.Open))
            {
                watch.Start();
                var bigEndianHashBytes32 = xxH32.ComputeHash(fs);
                watch.Stop();
                var xxHash32Cost = watch.Elapsed.TotalMilliseconds;
                fs.Seek(0, SeekOrigin.Begin);
                watch.Reset();
                watch.Start();
                var bigEndianHashBytes64 = xxH64.ComputeHash(fs);
                watch.Stop();
                var xxHash64Cost = watch.Elapsed.TotalMilliseconds;
                fs.Seek(0, SeekOrigin.Begin);
                watch.Reset();
                watch.Start();
                var md5HashBytes = md5.ComputeHash(fs);
                watch.Stop();
                var md5Cost = watch.Elapsed.TotalMilliseconds;

                fs.Seek(0, SeekOrigin.Begin);
                watch.Reset();
                watch.Start();
                var crc32NormalHashBytes = Crc32Normal.ComputeHash(fs);
                watch.Stop();
                var crc32NormalCost = watch.Elapsed.TotalMilliseconds;
#if SSE42
                fs.Seek(0, SeekOrigin.Begin);
                watch.Reset();
                watch.Start();
                var crc32SSEHashBytes = Crc32SSE.ComputeHash(fs);
                watch.Stop();
                var crc32SSECost = watch.Elapsed.TotalMilliseconds;
#endif
                Console.WriteLine($"My xxHash32 uint value: {xxH32.HashUInt32.ToString("x")}");
                Console.WriteLine($"My xxHash64 uint value: {xxH64.HashUInt64.ToString("x")}");
                Console.WriteLine();
                Console.WriteLine($"My xxHash32 bytes value(big endian): {BitConverter.ToString(bigEndianHashBytes32, 0, bigEndianHashBytes32.Length).Replace("-", "")}");
                Console.WriteLine($"My xxHash64 bytes value(big endian): {BitConverter.ToString(bigEndianHashBytes64, 0, bigEndianHashBytes64.Length).Replace("-", "")}");
                Console.WriteLine();
                Console.WriteLine($"MD5 bytes value: {BitConverter.ToString(md5HashBytes, 0, md5HashBytes.Length).Replace("-", "")}");
                Console.WriteLine($"CRC32C normal version bytes value: {BitConverter.ToString(crc32NormalHashBytes, 0, crc32NormalHashBytes.Length).Replace("-", "")}");
#if SSE42
                Console.WriteLine($"CRC32C SSE42 version bytes value: {BitConverter.ToString(crc32SSEHashBytes, 0, crc32SSEHashBytes.Length).Replace("-", "")}");
#endif
                Console.WriteLine();
                Console.WriteLine($"My xxHash32 cost: {xxHash32Cost.ToString()}");
                Console.WriteLine($"My xxHash64 cost: {xxHash64Cost.ToString()}");
                Console.WriteLine($"MD5 cost: {md5Cost.ToString()}");
                Console.WriteLine($"CRC32C normal version cost: {crc32NormalCost.ToString()}");
                XH32Cost += (decimal)xxHash32Cost;
                XH64Cost += (decimal)xxHash64Cost;
                MD5Cost += (decimal)md5Cost;
                CRCNormalCost += (decimal)crc32NormalCost;
#if SSE42
                Console.WriteLine($"CRC32C SSE42 version cost: {crc32SSECost.ToString()}");
                CRCSSECost += (decimal)crc32SSECost;
#endif
            }
            File.Delete(path);
        }

        private static void TestBytes(int index, int bytesSize)
        {
            TotalSize += (ulong)bytesSize;
            var buffer = new byte[bytesSize];
            unsafe
            {
                fixed (byte* buf = buffer)
                {
                    for (int i = 0; i < bytesSize; i++)
                    {
                        buf[i] = (byte)MyRnd.Next(0, 256);
                    }
                }
            }
            Console.WriteLine("=============================================================");
            Console.WriteLine($"No.{index}, Bytes size = {bytesSize.ToString("X")}");
            Console.WriteLine();
            watch.Start();
            var bigEndianHashBytes32 = xxH32.ComputeHash(buffer);
            watch.Stop();
            var xxHash32Cost = watch.Elapsed.TotalMilliseconds;
            watch.Reset();
            watch.Start();
            var bigEndianHashBytes64 = xxH64.ComputeHash(buffer);
            watch.Stop();
            var xxHash64Cost = watch.Elapsed.TotalMilliseconds;

            watch.Reset();
            watch.Start();
            var md5HashBytes = md5.ComputeHash(buffer);
            watch.Stop();
            var md5Cost = watch.Elapsed.TotalMilliseconds;
            watch.Reset();
            watch.Start();
            var crc32NormalHashBytes = Crc32Normal.ComputeHash(buffer);
            watch.Stop();
            var crc32NormalCost = watch.Elapsed.TotalMilliseconds;
#if SSE42
            watch.Reset();
            watch.Start();
            var crc32SSEHashBytes = Crc32SSE.ComputeHash(buffer);
            watch.Stop();
            var crc32SSECost = watch.Elapsed.TotalMilliseconds;
#endif
            Console.WriteLine($"My xxHash32 uint value: {xxH32.HashUInt32.ToString("x")}");
            Console.WriteLine($"My xxHash64 uint value: {xxH64.HashUInt64.ToString("x")}");
            Console.WriteLine();
            Console.WriteLine($"My xxHash32 bytes value(big endian): {BitConverter.ToString(bigEndianHashBytes32, 0, bigEndianHashBytes32.Length).Replace("-", "")}");
            Console.WriteLine($"My xxHash64 bytes value(big endian): {BitConverter.ToString(bigEndianHashBytes64, 0, bigEndianHashBytes64.Length).Replace("-", "")}");
            Console.WriteLine();
            Console.WriteLine($"MD5 bytes value: {BitConverter.ToString(md5HashBytes, 0, md5HashBytes.Length).Replace("-", "")}");
            Console.WriteLine($"CRC32C normal version bytes value: {BitConverter.ToString(crc32NormalHashBytes, 0, crc32NormalHashBytes.Length).Replace("-", "")}");
#if SSE42
            Console.WriteLine($"CRC32C SSE42 version bytes value: {BitConverter.ToString(crc32SSEHashBytes, 0, crc32SSEHashBytes.Length).Replace("-", "")}");
#endif
            Console.WriteLine();
            Console.WriteLine($"My xxHash32 cost: {xxHash32Cost.ToString()}");
            Console.WriteLine($"My xxHash64 cost: {xxHash64Cost.ToString()}");
            Console.WriteLine($"MD5 cost: {md5Cost.ToString()}");
            Console.WriteLine($"CRC32C normal version cost: {crc32NormalCost.ToString()}");
            XH32Cost += (decimal)xxHash32Cost;
            XH64Cost += (decimal)xxHash64Cost;
            MD5Cost += (decimal)md5Cost;
            CRCNormalCost += (decimal)crc32NormalCost;
#if SSE42
            Console.WriteLine($"CRC32C SSE42 version cost: {crc32SSECost.ToString()}");
            CRCSSECost += (decimal)crc32SSECost;
#endif

        }


        /*
         * Here we use official commandline tool xxhsum to check the C# port implementation.
         * "xxhsum -H& path", & is 0 or 1, 0=xxHash32, 1=xxHash64.
        */
        private static string GetXXHashCPortOutput(string path, int algorithmType)
        {
            string output;
            using (var proc = new Process())
            {
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.FileName = @".\OfficialCPortBinary\xxhsum.exe";
                proc.StartInfo.Arguments = $" -H{algorithmType} {path}";
                proc.Start();
                output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
            }
            return output.Substring(0, 8 + (8 * algorithmType));
        }



        private static string CreateFile(int fileSize)
        {
            var path = $"TestFile{Guid.NewGuid().ToString().Replace("-", "")}";
            using (var fs = new FileStream(path, FileMode.CreateNew))
            {
                var buffer = new byte[fileSize];
                unsafe
                {
                    fixed (byte* buf = buffer)
                    {
                        for (int i = 0; i < fileSize; i++)
                        {
                            buf[i] = (byte)MyRnd.Next(0, 256);
                        }
                    }
                }

                fs.Write(buffer, 0, fileSize);
            }
            return path;
        }

    }
}
