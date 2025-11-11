using System.Security.Cryptography;
using passwdManager.Services;
using System.Text;

namespace passwdManager.Services;

public class MasterPasswordService
{
    private readonly string hashPath = "Data/master.hash";

    public bool MasterPasswordExists()
    {
        if (File.Exists(hashPath))
        {
            if (new FileInfo(hashPath).Length > 0)
                return true;
        }
        return false;
    }

    public void CreateMasterPassword(string masterPassword)
    {
        var salt = GenerateSalt();
        var hash = HashPassword(masterPassword, salt);
        File.WriteAllBytes(hashPath, salt.Concat(hash).ToArray());
    }

    public void ChangeMasterPassword()
    {
        Console.Write("Enter old master password: ");
        string oldPassword = AccountManagementService.ReadPassword();

        if (!VerifyMasterPassword(oldPassword))
            throw new UnauthorizedAccessException("Old master password is incorrect.");

        Console.Write("Enter new master password: ");
        string newPassword = AccountManagementService.ReadPassword();

        var salt = GenerateSalt();
        var hash = HashPassword(newPassword, salt);
        File.WriteAllBytes(hashPath, salt.Concat(hash).ToArray());

        // confirm the change
        Console.Write("Confirm new master password: ");
        string confirmPassword = AccountManagementService.ReadPassword();

        if (newPassword != confirmPassword)
        {
            Console.WriteLine("Passwords do not match.");
            Console.WriteLine("Press [Enter] to continue...");
            Console.ReadLine();
            return;
        }
        Console.WriteLine("Master password changed successfully.");
        Console.WriteLine("Press [Enter] to continue...");
        Console.ReadLine();
    }

    public bool VerifyMasterPassword(string masterPassword)
    {
        if (!File.Exists(hashPath)) return false;

        byte[] data = File.ReadAllBytes(hashPath);
        byte[] salt = data[..16];
        byte[] storedHash = data[16..];

        byte[] computedHash = HashPassword(masterPassword, salt);
        return storedHash.SequenceEqual(computedHash);
    }

    private static byte[] GenerateSalt()
    {
        byte[] salt = new byte[16];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(32);
    }
}
