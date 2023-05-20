using System.Security.Cryptography;
using System.Text;

namespace Login.Factory;

public class SHA256Hash 
{
        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }
            }
            return sb.ToString();
        }
}
