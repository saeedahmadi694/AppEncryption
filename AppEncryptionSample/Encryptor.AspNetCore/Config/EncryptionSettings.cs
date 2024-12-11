namespace Encryptor.Config;

public class EncryptionSettings
{
    public const string Key = "Encryption";
    public Vault Vault { get; set; }
    public List<string> SensitiveColumns { get; set; } = new();
}
public class Vault
{
    public const string Key = "Vault";
    public string? Address { get; set; }
    public string? Token { get; set; }
    public string? EncryptionKey { get; set; }
}