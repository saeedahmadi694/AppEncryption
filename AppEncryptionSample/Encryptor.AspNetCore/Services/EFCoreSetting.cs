using Encryptor.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Encryptor.AspNetCore.Services;

public class EFCoreSetting : IEFCoreSetting
{
    private readonly VaultService _vaultService;
    private readonly EncryptionSettings _encryptionSettings;

    public EFCoreSetting(VaultService vaultService)
    {
        _vaultService = vaultService;
    }


    public async Task EncryptSensitiveColumnsAsync(ChangeTracker changeTracker)
    {
        foreach (var entry in changeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            var entityType = entry.Entity.GetType().Name;
            var column = _encryptionSettings.SensitiveColumns.FirstOrDefault(r => r == entityType);
            if (!string.IsNullOrEmpty(column))
            {
                var property = entry.Property(column);
                if (property != null && property.CurrentValue is string currentValue)
                {
                    property.CurrentValue = await _vaultService.EncryptAsync(currentValue);
                }

            }
        }
    }

    public async Task<TEntity> DecryptItemAsync<TEntity>(TEntity entity)
    {
        var entityType = entity.GetType().Name;
        var column = _encryptionSettings.SensitiveColumns.FirstOrDefault(r => r == entityType);
        if (!string.IsNullOrEmpty(column))
        {
            var property = entity.GetType().GetProperty(column);
            if (property != null && property.GetValue(entity) is string encryptedValue)
            {
                var decryptedValue = await _vaultService.DecryptAsync(encryptedValue);
                property.SetValue(entity, decryptedValue);
            }
        }
        return entity;
    }
}
