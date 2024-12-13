using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Encryptor.AspNetCore.Services;

public class TransitService : ITransitService
{
    private readonly HttpClient _httpClient;
    private readonly string _encryptionKey;
    private readonly string _path;

    public TransitService(string encryptionKey, string vaultToken, string baseAddress, string path)
    {
        _encryptionKey = encryptionKey;
        _httpClient = new()
        {
            BaseAddress = new Uri(baseAddress),
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", vaultToken);
        _path = path;
    }

    public async Task<string> EncryptAsync(string plaintext)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { plaintext = Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext)) }), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"/v1/{_path}/encrypt/{_encryptionKey}", content);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JsonElement>(result).GetProperty("data").GetProperty("ciphertext").GetString();
    }

    public async Task<string> DecryptAsync(string ciphertext)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { ciphertext }), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"/v1/{_path}/decrypt/{_encryptionKey}", content);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        var plaintext = JsonSerializer.Deserialize<JsonElement>(result).GetProperty("data").GetProperty("plaintext").GetString();
        return Encoding.UTF8.GetString(Convert.FromBase64String(plaintext));
    }
}


