using Encryptor.Config;
using System.Data.Entity;

namespace Encryptor.Services
{
    public class EFCoreSetting : IEFCoreSetting
    {
        private readonly VaultService _vaultService;
        private readonly VaultSettings _encryptionSettings;

        public EFCoreSetting(VaultService vaultService, DbContextOptions<T> option) : base(option)
        {
            _vaultService = vaultService;
            _encryptionSettings = encryptionSettings.Value;
        }

        private void EncryptSensitiveColumns()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified))
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

        private async Task EncryptSensitiveColumnsAsync()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified))
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

        public override DbSet<TEntity> Set<TEntity>()
        {
            var queryable = base.Set<TEntity>();
            var entityType = typeof(TEntity).Name;

            if (_encryptionSettings.SensitiveData.ContainsKey(entityType))
            {
                queryable = queryable.AsEnumerable()
                    .Select(async entity =>
                    {
                        foreach (var column in _encryptionSettings.SensitiveData[entityType])
                        {
                            var property = entity.GetType().GetProperty(column);
                            if (property != null && property.GetValue(entity) is string encryptedValue)
                            {
                                var decryptedValue = await _vaultService.DecryptAsync(encryptedValue);
                                property.SetValue(entity, decryptedValue);
                            }
                        }
                        return entity;
                    }).Select(task => task.Result).AsQueryable();
            }

            return queryable;
        }

    }

}
