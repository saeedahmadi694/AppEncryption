using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Encryptor.Services;

public class VaultService : IVaultService
{
    private readonly HttpClient? _httpClient;
    private readonly string _encryptionKey;

    public VaultService(string encryptionKey, string vaultToken, string baseAddress)
    {
        _encryptionKey = encryptionKey;
        _httpClient = new()
        {
            BaseAddress = new Uri(baseAddress),
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", vaultToken);
    }

    public async Task<string> EncryptAsync(string plaintext)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { plaintext = Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext)) }), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"/v1/transit/encrypt/{_encryptionKey}", content);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JsonElement>(result).GetProperty("data").GetProperty("ciphertext").GetString();
    }

    public async Task<string> DecryptAsync(string ciphertext)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { ciphertext }), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"/v1/transit/decrypt/{_encryptionKey}", content);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        var plaintext = JsonSerializer.Deserialize<JsonElement>(result).GetProperty("data").GetProperty("plaintext").GetString();
        return Encoding.UTF8.GetString(Convert.FromBase64String(plaintext));
    }
}
public class EncryptionProcessor
{
    private readonly VaultService _vaultService;
    private readonly EncryptionSettings _encryptionSettings;

    public EncryptionProcessor(VaultService vaultService, IOptions<EncryptionSettings> encryptionSettings)
    {
        _vaultService = vaultService;
        _encryptionSettings = encryptionSettings.Value;
    }

    public void EncryptSensitiveColumns(ChangeTracker changeTracker)
    {
        foreach (var entry in changeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
        {
            var entityType = entry.Entity.GetType().Name;
            if (_encryptionSettings.Columns.ContainsKey(entityType))
            {
                foreach (var column in _encryptionSettings.Columns[entityType])
                {
                    var property = entry.Property(column);
                    if (property != null && property.CurrentValue is string currentValue)
                    {
                        property.CurrentValue = _vaultService.EncryptAsync(currentValue).Result;
                    }
                }
            }
        }
    }

    public async Task EncryptSensitiveColumnsAsync(ChangeTracker changeTracker)
    {
        foreach (var entry in changeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
        {
            var entityType = entry.Entity.GetType().Name;
            if (_encryptionSettings.Columns.ContainsKey(entityType))
            {
                foreach (var column in _encryptionSettings.Columns[entityType])
                {
                    var property = entry.Property(column);
                    if (property != null && property.CurrentValue is string currentValue)
                    {
                        property.CurrentValue = await _vaultService.EncryptAsync(currentValue);
                    }
                }
            }
        }
    }

    public async Task<TEntity> DecryptItemAsync<TEntity>(TEntity entity)
    {
        var entityType = entity.GetType().Name;
        if (_encryptionSettings.Columns.ContainsKey(entityType))
        {
            foreach (var column in _encryptionSettings.Columns[entityType])
            {
                var property = entity.GetType().GetProperty(column);
                if (property != null && property.GetValue(entity) is string encryptedValue)
                {
                    var decryptedValue = await _vaultService.DecryptAsync(encryptedValue);
                    property.SetValue(entity, decryptedValue);
                }
            }
        }
        return entity;
    }
}