using System;
using System.Security.Cryptography;

namespace HybridCryptoSample
{
    public static class SymmetricKeyGen
    {
        public static byte[] GenerateAesKey(int keySizeBits = 256)
        {
            using var aes = Aes.Create();
            aes.KeySize = keySizeBits;
            aes.GenerateKey();
            return aes.Key;
        }

        // Example: get a Base64 string to store or transmit
        public static string GenerateAesKeyBase64(int keySizeBits = 256)
        {
            var key = GenerateAesKey(keySizeBits);
            return Convert.ToBase64String(key);
        }
    }
}
