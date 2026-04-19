using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Library_Business
{
    public class clsUtil
    {
        /// <summary>
        /// This method generates a hexadecimal hash string of the input.
        /// </summary>
        /// <param name="input">The string to be hashed.</param>
        /// <returns>The hash signature of the input.</returns>
        public static string ComputeHash(string input)
        {
            using (SHA256 sHA256 = SHA256.Create())
            {
                StringBuilder sb = new StringBuilder();
                Byte[] hashBytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(input));
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
