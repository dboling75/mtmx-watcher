# Hybrid Crypto Sample (C#)

This zip contains:
- `src/SymmetricKeyGen.cs` — lightweight AES key generator (128-bit or 256-bit).
- `src/HybridEncryptor.cs` — hybrid encryption helper that:
  1. Encrypts plaintext with **AES‑GCM** using your symmetric key.
  2. Encrypts that symmetric key with an **RSA public key (PEM)** using **OAEP‑SHA256**.
  3. Returns an envelope with `EncryptedKeyB64`, `NonceB64`, `TagB64`, and `CiphertextB64`.

> Target runtime: .NET 6+ (works in Azure Functions). No extra NuGet packages required.

---

## Quick Start

### 1) Generate a symmetric key
```csharp
using HybridCryptoSample;

// 256‑bit key (recommended)
byte[] aesKey = SymmetricKeyGen.GenerateAesKey(256);

// or as Base64 for storage/transport
string aesKeyB64 = SymmetricKeyGen.GenerateAesKeyBase64(256);
```

### 2) Load your RSA public key (PEM)
You can pass either a public key PEM or a certificate PEM containing the public key.
```csharp
string publicKeyPem = File.ReadAllText("/path/to/public-key.pem");
// Example PEM header expected:
// -----BEGIN PUBLIC KEY-----
// MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A...
// -----END PUBLIC KEY-----
```

### 3) Encrypt plaintext (hybrid)
```csharp
using HybridCryptoSample;

string plaintext = "Hello, world!";
var envelope = HybridEncryptor.Encrypt(publicKeyPem, aesKey, plaintext);

// Or get compact JSON (suitable to send over the wire)
string envelopeJson = HybridEncryptor.EncryptToJson(publicKeyPem, aesKey, plaintext);
Console.WriteLine(envelopeJson);
```
**Envelope fields:**
- `KeyAlgorithm`: `RSA-OAEP-256`
- `EncAlgorithm`: `A256GCM`
- `EncryptedKeyB64`: RSA-encrypted AES key (Base64)
- `NonceB64`: 12‑byte AES‑GCM nonce (Base64)
- `TagB64`: 16‑byte AES‑GCM tag (Base64)
- `CiphertextB64`: AES‑GCM ciphertext (Base64)

> **Note:** To decrypt you will need the RSA **private key** (to unwrap the AES key) and then AES‑GCM with the same `NonceB64` and `TagB64` to recover the plaintext.

---

## Usage in Azure Functions

- Place the `src/*.cs` files in your Function project (e.g., under a `Shared` folder or a class library).
- Ensure your project targets .NET 6 or .NET 8 so `AesGcm` is available.
- If your RSA key lives in **Azure Key Vault**, you can fetch the **public key** material and still use this helper for envelope encryption. (For end‑to‑end managed crypto, consider `CryptographyClient` in `Azure.Security.KeyVault.Keys`.)

---

## Security Notes

- Prefer **AES‑256‑GCM** (as used here) for authenticated encryption.
- Always generate a **fresh random nonce** per message (done automatically here).
- Do **not** reuse nonce+key pairs.
- Use **RSA‑OAEP‑SHA256** rather than RSA1_5 for key wrapping.
- Consider key rotation and storage best practices (e.g., Key Vault, HSMs).
