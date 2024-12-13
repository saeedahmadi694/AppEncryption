using Encryptor.AspNetCore.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;

namespace Encryptor.AspNetCore.Services;

public class EFCoreSetting : IEFCoreSetting
{
    private readonly ITransitService _transitService;
    private readonly EncryptionSetting _encryptionSetting;

    public EFCoreSetting(EncryptionSetting encryptionSetting, ITransitService transitService)
    {
        _encryptionSetting = encryptionSetting;
        _transitService = transitService;
    }


   
    public void DecryptSensitiveColumns(object entity)
    {
        var entityType = entity.GetType();
        var sensitiveProperties = entityType.GetProperties()
            .Where(p => _encryptionSetting.SensitiveColumns.Contains(p.Name));

        foreach (var property in sensitiveProperties)
        {
            var value = property.GetValue(entity) as string;
            if (!string.IsNullOrEmpty(value))
            {
                var decryptedValue = _transitService.DecryptAsync(value).Result; 
                property.SetValue(entity, decryptedValue);
            }
        }
    }

    public void EncryptSensitiveColumns(ChangeTracker changeTracker)
    {
        foreach (var entry in changeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
        {
            var entityType = entry.Entity.GetType();
            var sensitiveProperties = entityType.GetProperties()
                .Where(p => _encryptionSetting.SensitiveColumns.Contains(p.Name));

            foreach (var property in sensitiveProperties)
            {
                var currentValue = entry.Property(property.Name).CurrentValue as string;
                if (!string.IsNullOrEmpty(currentValue))
                {
                    entry.Property(property.Name).CurrentValue = _transitService.EncryptAsync(currentValue).Result;
                }
            }
        }
    }

    public async Task EncryptSensitiveColumnsAsync(ChangeTracker changeTracker)
    {
        foreach (var entry in changeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
        {
            var entityType = entry.Entity.GetType();
            var sensitiveProperties = entityType.GetProperties()
                .Where(p => _encryptionSetting.SensitiveColumns.Contains(p.Name));

            foreach (var property in sensitiveProperties)
            {
                var currentValue = entry.Property(property.Name).CurrentValue as string;
                if (!string.IsNullOrEmpty(currentValue))
                {
                    entry.Property(property.Name).CurrentValue = await _transitService.EncryptAsync(currentValue);
                }
            }
        }
    }

}
