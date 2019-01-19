using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SubtitleDownloadCore
{
    public class FileHashUtil
    {
        private static readonly object fsLock = new object();

        public static string GetSubdbFileHash(string filePath)
        {
            int bufferSize = 64 * 1024;
            byte[] first64kb = new byte[bufferSize];
            byte[] last64kb = new byte[bufferSize];

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                lock (fsLock)
                {
                    // first 64k
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Read(first64kb, 0, bufferSize);

                    // last 64k
                    fs.Seek(-bufferSize, SeekOrigin.End);
                    fs.Read(last64kb, 0, bufferSize);
                }
            }

            using (var md5 = MD5.Create())
            {
                byte[] concatBytes = first64kb.Concat(last64kb).ToArray();
                var hash = md5.ComputeHash(concatBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }

        }
    }
}
