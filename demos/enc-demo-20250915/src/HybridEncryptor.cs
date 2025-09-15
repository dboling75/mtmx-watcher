using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace HybridCryptoSample
{
    /// <summary>
    /// Performs hybrid encryption:
    /// 1) Encrypt plaintext with AES-GCM using a provided symmetric key.
    /// 2) Encrypt that symmetric key with an RSA public key (PEM) using OAEP-SHA256.
    /// Returns an envelope with ciphertext, nonce (IV), tag, and encrypted key.
    /// </summary>
    public static class HybridEncryptor
    {
        public sealed class EncryptedEnvelope
        {
            public string KeyAlgorithm { get; init; } = "RSA-OAEP-256";
            public string EncAlgorithm { get; init; } = "A256GCM";
            public string EncryptedKeyB64 { get; init; } = string.Empty; // RSA-encrypted AES key
            public string NonceB64 { get; init; } = string.Empty;        // 12 bytes, AES-GCM nonce
            public string TagB64 { get; init; } = string.Empty;          // 16 bytes, AES-GCM tag
            public string CiphertextB64 { get; init; } = string.Empty;   // ciphertext
        }

        /// <summary>
        /// Encrypts the plaintext using AES-GCM with the supplied symmetricKey,
        /// and encrypts symmetricKey with the RSA public key in PEM format.
        /// </summary>
        public static EncryptedEnvelope Encrypt(string rsaPublicKeyPem, byte[] symmetricKey, string plaintext)
        {
            if (symmetricKey == null || (symmetricKey.Length != 16 && symmetricKey.Length != 32))
                throw new ArgumentException("symmetricKey must be 128-bit (16 bytes) or 256-bit (32 bytes).");

            // 1) AES-GCM encrypt the plaintext
            byte[] nonce = RandomNumberGenerator.GetBytes(12); // 96-bit nonce recommended for GCM
            byte[] tag = new byte[16]; // 128-bit tag
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] ciphertext = new byte[plaintextBytes.Length];

            using (var aesGcm = new AesGcm(symmetricKey))
            {
                aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);
            }

            // 2) RSA-OAEP-256 encrypt the symmetric key
            byte[] encryptedKey;
            using (var rsa = RSA.Create())
            {
                rsa.ImportFromPem(rsaPublicKeyPem.AsSpan());
                encryptedKey = rsa.Encrypt(symmetricKey, RSAEncryptionPadding.OaepSHA256);
            }

            return new EncryptedEnvelope
            {
                KeyAlgorithm = "RSA-OAEP-256",
                EncAlgorithm = "A256GCM",
                EncryptedKeyB64 = Convert.ToBase64String(encryptedKey),
                NonceB64 = Convert.ToBase64String(nonce),
                TagB64 = Convert.ToBase64String(tag),
                CiphertextB64 = Convert.ToBase64String(ciphertext)
            };
        }

        /// <summary>
        /// Convenience helper: produce a compact JSON string of the envelope.
        /// </summary>
        public static string EncryptToJson(string rsaPublicKeyPem, byte[] symmetricKey, string plaintext)
        {
            var env = Encrypt(rsaPublicKeyPem, symmetricKey, plaintext);
            var json = JsonSerializer.Serialize(env, new JsonSerializerOptions
            {
                WriteIndented = false
            });
            return json;
        }
    }
}
