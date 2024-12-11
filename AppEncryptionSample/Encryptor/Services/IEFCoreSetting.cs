using Encryptor.Config;
using System.Data.Entity;

namespace Encryptor.Services
{
    public interface IEFCoreSetting
    {
        void EncryptSensitiveColumns();
        Task EncryptSensitiveColumnsAsync();
        DbSet<TEntity> Set<TEntity>();

    }

}
