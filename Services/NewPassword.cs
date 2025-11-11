namespace passwdManager.Services;

class PasswordHelper
{
    public static bool IsStrong(string password)
    {
        if (password.Length < 12)
            return false;

        bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;

        foreach (char c in password)
        {
            switch (c)
            {
                case char _ when char.IsUpper(c):
                    hasUpper = true;
                    break;
                case char _ when char.IsLower(c):
                    hasLower = true;
                    break;
                case char _ when char.IsDigit(c):
                    hasDigit = true;
                    break;
                default:
                    hasSpecial = true;
                    break;
            }

            if (hasUpper && hasLower && hasDigit && hasSpecial)
                break;
        }

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }

    public static string GeneratePassword()
    {
        string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string lower = "abcdefghijklmnopqrstuvwxyz";
        string digits = "0123456789";
        string special = "!@#$%^&*";
        string allChars = upper + lower + digits + special;

        Random rand = new Random();
        char[] pwd = new char[16];

        for (int i = 0; i < pwd.Length; i++)
        {
            pwd[i] = allChars[rand.Next(allChars.Length)];
        }
        var password = new string(pwd);

        while (!IsStrong(password))
        {
            for (int i = 0; i < pwd.Length; i++)
            {
                pwd[i] = allChars[rand.Next(allChars.Length)];
            }
            password = new string(pwd);
        }
        
        return password;
    }

}