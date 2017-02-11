using System.Security.Cryptography;
using System.Text;

namespace Poster.Core
{
    public static class HashExtensions
    {
        public static byte[] ToSha1Hash(this string data, Encoding encoding = null)
        {
            encoding = encoding ?? Default.Encoding;

            using (SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider())
            {
                return hasher.ComputeHash(encoding.GetBytes(data));
            }
        }

        /// <summary>
        /// Better than BitConverter.ToString because we don't get hyphens and we can ask for lowercase (matching non-.NET hash string implementations)
        /// http://stackoverflow.com/questions/2435695/converting-a-md5-hash-byte-array-to-a-string
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="upperCase"></param>
        /// <returns></returns>
        public static string ToHex(this byte[] bytes, bool upperCase = false)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            }

            return result.ToString();
        }
    }
}
