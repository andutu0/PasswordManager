namespace passwdManager.Stored;

public class Credential
{
    public string Website { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;
}
