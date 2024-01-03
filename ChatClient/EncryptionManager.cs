using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public static class EncryptionManager
    {
        // Convert BigInteger to a byte array of a specified size
        private static byte[] BigIntegerToBytes(BigInteger bigInt, int size)
        {
            byte[] fullArray = bigInt.ToByteArray();
            if (fullArray.Length == size)
            {
                return fullArray;
            }

            byte[] result = new byte[size];
            Array.Copy(fullArray, result, Math.Min(fullArray.Length, size));
            return result;
        }

        // Encrypt a string using AES encryption.
        public static string Encrypt(string text, BigInteger keyBigInt, byte[] iv)
        {
            byte[] encrypted;

            // Convert BigInteger to byte array for the key
            byte[] key = BigIntegerToBytes(keyBigInt, 16); // 128 bits for AES key

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        // Decrypt a string using AES encryption.
        public static string Decrypt(string cipherText, BigInteger keyBigInt, byte[] iv)
        {
            byte[] buffer = Convert.FromBase64String(cipherText);

            // Convert BigInteger to byte array for the key
            byte[] key = BigIntegerToBytes(keyBigInt, 16); // 128 bits for AES key

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(buffer))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }

}
