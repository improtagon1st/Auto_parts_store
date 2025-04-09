using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Auto_parts_store
{
    internal static class HashHelper
    {
        public static string GetHash(string password)
        {
            using (var sha = SHA1.Create())
            {
                return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(password))
                    .Select(b => b.ToString("X2")));
            }
        }
    }
}
