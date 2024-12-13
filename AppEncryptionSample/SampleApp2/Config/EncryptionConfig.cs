namespace Encryptor.AspNetCore.Config;

public class EncryptionConfig
{
    public const string Key = "Encryption";
    public VaultConfig? Vault { get; set; }
    public List<string> SensitiveColumns { get; set; } = new();
}
public class VaultConfig
{
    public string? BaseAddress { get; set; }
    public string? Token { get; set; }
    public TransitConfig? Transit { get; set; }
}

public class TransitConfig
{
    public string? Path { get; set; }
    public string? EncryptionKey { get; set; }
}