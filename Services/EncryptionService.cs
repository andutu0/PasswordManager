using System.Security.Cryptography;
using System.Text;

namespace passwdManager.Services;

public class EncryptionService
{
    private byte[] key;

    public EncryptionService(string masterPassword)
    {
        using var sha256 = SHA256.Create();
        key = SHA256.HashData(Encoding.UTF8.GetBytes(masterPassword));
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();
        using var encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipher = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(aes.IV.Concat(cipher).ToArray());
    }

    public string Decrypt(string cipherText)
    {
        byte[] bytes = Convert.FromBase64String(cipherText);
        byte[] iv = bytes[..16];
        byte[] cipher = bytes[16..];

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor();
        byte[] plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}
