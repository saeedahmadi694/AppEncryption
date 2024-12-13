using System.Text;
using System.Text.Json;

namespace Encryptor.AspNetCore.Services;

public interface ITransitService
{
    Task<string> EncryptAsync(string plaintext);
    Task<string> DecryptAsync(string ciphertext);
}
