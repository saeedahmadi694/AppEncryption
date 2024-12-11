using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Encryptor.AspNetCore.Services;

public interface IEFCoreSetting
{
    Task EncryptSensitiveColumnsAsync(ChangeTracker changeTracker);

}
