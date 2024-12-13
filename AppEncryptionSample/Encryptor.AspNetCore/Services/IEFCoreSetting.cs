using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Encryptor.AspNetCore.Services;

public interface IEFCoreSetting
{
    Task EncryptSensitiveColumnsAsync(ChangeTracker changeTracker);
    void EncryptSensitiveColumns(ChangeTracker changeTracker);
    void DecryptSensitiveColumns(object entity);
}
