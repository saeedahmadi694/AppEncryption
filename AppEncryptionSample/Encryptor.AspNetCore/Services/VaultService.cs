using Encryptor.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Encryptor.AspNetCore.Services;

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


