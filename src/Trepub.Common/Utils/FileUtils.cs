using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Trepub.Common.Utils
{
    public class FileUtils
    {
        public static string CalcFileMD5sum(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "‌​").ToLower();
                }
            }
        }
        public static string CalcStringMD5sum(string str)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(str))).Replace("-", "‌​").ToLower();
            }
        }
        public static void MoveFileSafe(string src, string dst, string type)
        {
            if (File.Exists(dst))
            {
                File.Move(src, Path.Combine(dst + "_" + Guid.NewGuid()));
            }
            else
            {
                File.Move(src, dst);
            }
        }
    }
}
