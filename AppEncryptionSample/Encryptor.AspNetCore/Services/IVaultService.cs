using System.Text;
using System.Text.Json;

namespace Encryptor.Services;

public interface IVaultService
{
    Task<string> EncryptAsync(string plaintext);
    Task<string> DecryptAsync(string ciphertext);
}
