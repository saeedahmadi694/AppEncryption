namespace Encryptor.AspNetCore.Config;

public class EncryptionSetting
{
    public VaultSetting? Vault { get; set; }
    public List<string> SensitiveColumns { get; set; } = new();
}
public class VaultSetting
{
    public string? BaseAddress { get; set; }
    public string? Token { get; set; }
    public TransitSetting? Transit { get; set; }
}

public class TransitSetting
{
    public string? Path { get; set; }
    public string? EncryptionKey { get; set; }
}