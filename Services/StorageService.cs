using passwdManager.Stored;
using System.Text.Json;

namespace passwdManager.Services;

public class StorageService
{
    private readonly string path = "Data/vault.json";
    
    public List<Credential> Load()
    {
        if (!File.Exists(path)) return new List<Credential>();
        string json = File.ReadAllText(path).Trim();
        if (string.IsNullOrEmpty(json))
            return new List<Credential>();

        return JsonSerializer.Deserialize<List<Credential>>(json) ?? new List<Credential>();
    }

    public void Save(List<Credential> credentials)
    {
        Directory.CreateDirectory("Data");
        string json = JsonSerializer.Serialize(credentials, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

}
