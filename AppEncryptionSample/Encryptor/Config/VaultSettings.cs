namespace Encryptor.Config;

public class VaultSettings
{
    public const string Key = "Vault";
    public string? Address { get; set; }
    public string? Token { get; set; }
    public string? EncryptionKey { get; set; }
    public Dictionary<string, List<string>> SensitiveData { get; set; } = [];
}