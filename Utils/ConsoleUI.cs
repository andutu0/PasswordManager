using passwdManager.Stored;
using passwdManager.Services;

namespace passwdManager.Utils;

public class ConsoleUI
{
    private StorageService storage = new();
    private EncryptionService? crypto;
    private AccountManagementService? accountService;

    public void Run()
    {
        var master = new MasterPasswordService();
        string masterPassword;
        Console.Clear();

        if (!master.MasterPasswordExists())
        {
            Console.Write("A master password is necessary for managing your accounts.\nYour new master password will be: ");
            masterPassword = AccountManagementService.ReadPassword();
            master.CreateMasterPassword(masterPassword);
            Console.WriteLine("\nMaster password created successfully!\n");
        }
        else
        {
            bool ok = false;
            do
            {
                Console.Write("Your master password: ");
                masterPassword = AccountManagementService.ReadPassword();
                ok = master.VerifyMasterPassword(masterPassword);
                if (!ok) Console.WriteLine("\nIncorrect password. Try again!\n");
            } while (!ok);
        }

        crypto = new EncryptionService(masterPassword);
        accountService = new AccountManagementService(storage, crypto);
        Console.WriteLine("\nVault unlocked!\n");

        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Add account");
            Console.WriteLine("2. See accounts");
            Console.WriteLine("3. Manage accounts");
            Console.WriteLine("4. See an account's user and password");
            Console.WriteLine("5. Change master password");
            Console.WriteLine("6. Check password strength");
            Console.WriteLine("7. Generate new strong password");
            Console.WriteLine("8. Exit");
            Console.Write("> ");
            string choice = Console.ReadLine() ?? "";

            if (choice == "1")
            {
                Console.Clear();
                accountService.AddCredential();
            }
            else if (choice == "2")
            {
                Console.Clear();
                accountService.ListAccounts();
            }
            else if (choice == "3")
            {
                Console.Clear();
                Console.Write("Enter website: ");
                string website = Console.ReadLine() ?? "";
                accountService.ManageAccounts(website);
            }
            else if (choice == "4")
            {
                Console.Clear();
                Console.Write("Enter website: ");
                string website = Console.ReadLine() ?? "";
                Console.Write("Enter username: ");
                string username = Console.ReadLine() ?? "";
                accountService.ShowPassword(website, username);
            }
            else if (choice == "5")
            {
                Console.Clear();
                master.ChangeMasterPassword();
            }
            else if (choice == "6")
            {
                Console.Clear();
                int nrChecks = 0;
                while (true)
                {
                    if (nrChecks != 0)
                    {
                        Console.Write("Would you like to test another password? (y/n)\n");
                        char option = Console.ReadKey().KeyChar;
                        Console.WriteLine();
                        if (option == 'n' || option == 'N')
                            break;
                    }
                    Console.Write("Enter password to check: ");
                    string password_to_check = Console.ReadLine() ?? "";
                    if (PasswordHelper.IsStrong(password_to_check))
                    {
                        Console.Write("This password is strong!\n");
                    }
                    else
                    {
                        Console.Write("This password is NOT strong!\n");
                    }
                    nrChecks++;
                }
            }
            else if (choice == "7")
            {
                Console.Clear();
                int nrPasswords = 0;
                while (true)
                {
                    if (nrPasswords != 0)
                    {
                        Console.Clear();
                        Console.Write("Would you like to generate another password? (y/n)\n");
                        char option = Console.ReadKey().KeyChar;
                        Console.WriteLine();
                        if (option == 'n' || option == 'N')
                            break;
                    }
                    Console.Clear();
                    string newPassword = PasswordHelper.GeneratePassword();
                    Console.WriteLine($"Generated strong password: {newPassword}\n");
                    Console.WriteLine("Press [Enter] to continue...");
                    Console.ReadLine();
                    nrPasswords++;
                }
            }
            else if (choice == "8")
            {
                Environment.Exit(0);
            }
            else
                Console.WriteLine("Invalid choice. Try again.\n");
        }
    }
}
