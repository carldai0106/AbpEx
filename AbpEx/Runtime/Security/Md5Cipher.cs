using System;
using System.Security.Cryptography;
using System.Text;

namespace Abp.Runtime.Security
{
    public class Md5Cipher
    {
        public static string GetMd5Str(string source)
        {
            var md5 = new MD5CryptoServiceProvider();
            var str = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(source)), 4, 8);
            str = str.Replace("-", "");
            str = str.ToLower();
            return str;
        }
    }
}
