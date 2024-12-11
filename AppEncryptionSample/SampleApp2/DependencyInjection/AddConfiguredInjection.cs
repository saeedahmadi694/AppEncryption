using Encryptor.AspNetCore.Services;
using Encryptor.Config;
using Encryptor.Services;

namespace SampleApp2.DependencyInjection;

public static class AddConfiguredInjection
{
    public static IServiceCollection AddConfiguredVandar(this IServiceCollection services, IConfiguration configuration)
    {

        var minioOptions = new EncryptionSettings();
        minioOptions = configuration.GetSection(EncryptionSettings.Key).Get<EncryptionSettings>();
        services.AddSingleton<IVaultService>(sp =>
        {
            return new VaultService(minioOptions.Vault.EncryptionKey, minioOptions.Vault.Token, minioOptions.Vault.Address);
        });

        return services;
    }
}

