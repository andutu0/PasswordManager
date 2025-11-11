using passwdManager.Stored;
using System.Security.Cryptography;
using System.Text.Json;

namespace passwdManager.Services;

public class AccountManagementService
{
    private StorageService storage;
    private EncryptionService? crypto;

    public AccountManagementService(StorageService storageService, EncryptionService encryptionService)
    {
        storage = storageService;
        crypto = encryptionService;
    }

    public void AddCredential()
    {
        Console.Write("Website: ");
        string website = Console.ReadLine() ?? "";
        Console.Write("Username: ");
        string username = Console.ReadLine() ?? "";
        Console.Write("Password: ");
        string password = ReadPassword() ?? "";

        var creds = storage.Load();
        creds.Add(new Credential
        {
            Website = website,
            Username = username,
            EncryptedPassword = crypto!.Encrypt(password)
        });
        storage.Save(creds);
        Console.WriteLine("Account saved!");
        Console.WriteLine("Press [Enter] to continue...");
        Console.ReadLine();
    }

    public void ListAccounts()
    {
        var creds = storage.Load();
        Console.WriteLine("-------------------------------\n");
        foreach (var c in creds)
        {
            Console.WriteLine($"Website : {c.Website}\nUsername: {c.Username}\n");
            Console.WriteLine("-------------------------------\n");
        }
        Console.WriteLine("Press [Enter] to continue...");
        Console.ReadLine();
    }

    public void ManageAccounts(string website)
    {
        var creds = storage.Load();
        var filtered = creds.Where(c => c.Website.Equals(website, StringComparison.OrdinalIgnoreCase)).ToList(); ;
        Credential? account = null;

        if (filtered.Count == 0)
        {
            Console.WriteLine("No accounts found for this website.");
            return;
        }

        Console.WriteLine("Accounts found:");
        for (int i = 0; i < filtered.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {filtered[i].Username}");
        }

        Console.Write("Select an account (number): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= filtered.Count)
        {
            account = filtered[choice - 1];
        }
        else
        {
            Console.WriteLine("Invalid selection.");
            Console.WriteLine("Press [Enter] to continue...");
            Console.ReadLine();
            return;
        }
    

        Console.WriteLine("1. Update password");
        Console.WriteLine("2. Delete account");
        Console.Write("> ");

        string action = Console.ReadLine() ?? "";

        if (action == "1")
        {
            Console.Write("New password: ");
            string newPassword = ReadPassword() ?? "";
            account.EncryptedPassword = crypto!.Encrypt(newPassword);
            storage.Save(creds);
            Console.WriteLine("Password updated successfully!");
            Console.WriteLine("Press [Enter] to continue...");
            Console.ReadLine();
        }
        else if (action == "2")
        {
            creds.Remove(account);
            storage.Save(creds);
            Console.WriteLine("Account deleted successfully!");
            Console.WriteLine("Press [Enter] to continue...");
            Console.ReadLine();
        }
    }

    public void ShowPassword(string website, string username)
    {
        var creds = storage.Load();
        foreach (var c in creds)
        {
            if (c.Website.Equals(website, StringComparison.OrdinalIgnoreCase))
            {
                if (c.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        string decryptedPassword = crypto!.Decrypt(c.EncryptedPassword);
                        Console.WriteLine($"\nPassword for {username} at {website}: {decryptedPassword}\n");
                    }
                    catch (CryptographicException)
                    {
                        Console.WriteLine($"\nError: Unable to decrypt the password for {username} at {website}.");
                        Console.WriteLine("This usually means the master password is different or the vault data was corrupted.");
                    }

                    Console.WriteLine("Press [Enter] to continue...");
                    Console.ReadLine();
                    return;
                }
            }
        }
    }

    public static string ReadPassword()
    {
        var sb = new System.Text.StringBuilder();
        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                sb.Append(keyInfo.KeyChar);
                Console.Write("*");
            }
        } while (key != ConsoleKey.Enter);

        Console.WriteLine();
        return sb.ToString();
    }
}